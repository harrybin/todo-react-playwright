import { test as base } from '@playwright/test';

/**
 * Multi-Tenant Test Fixtures for Epic 1
 *
 * Provides:
 * - Two isolated tenant contexts
 * - Users bound to respective tenants
 * - API helpers for setup/validation
 * - Data cleanup
 */

export type TenantContext = {
  tenantId: string;
  tenantName: string;
  tenantDomain: string;
  users: TestUser[];
  tasks: TestTask[];
};

export type TestUser = {
  id: string;
  email: string;
  name: string;
  tenantId: string;
  role: 'admin' | 'user';
  idpDomain: string;
  mfaEnabled: boolean;
};

export type TestTask = {
  id: string;
  title: string;
  description: string;
  tenantId: string;
  userId: string;
  completed: boolean;
  createdAt: string;
};

/**
 * Fixture: tenantA
 * Test data for first enterprise customer
 */
const tenantA: TenantContext = {
  tenantId: 'tenant-001-acme',
  tenantName: 'ACME Corporation',
  tenantDomain: 'acme.example.com',
  users: [
    {
      id: 'user-001-alice',
      email: 'alice@acme.example.com',
      name: 'Alice Admin',
      tenantId: 'tenant-001-acme',
      role: 'admin',
      idpDomain: 'acme.example.com',
      mfaEnabled: true,
    },
    {
      id: 'user-002-bob',
      email: 'bob@acme.example.com',
      name: 'Bob User',
      tenantId: 'tenant-001-acme',
      role: 'user',
      idpDomain: 'acme.example.com',
      mfaEnabled: false,
    },
  ],
  tasks: [
    {
      id: 'task-001-acme',
      title: 'ACME Q2 Planning',
      description: 'Confidential strategic planning',
      tenantId: 'tenant-001-acme',
      userId: 'user-001-alice',
      completed: false,
      createdAt: '2026-05-01T10:00:00Z',
    },
    {
      id: 'task-002-acme',
      title: 'ACME Revenue Report',
      description: 'Financial data (SENSITIVE)',
      tenantId: 'tenant-001-acme',
      userId: 'user-002-bob',
      completed: false,
      createdAt: '2026-05-02T14:30:00Z',
    },
  ],
};

/**
 * Fixture: tenantB
 * Test data for second enterprise customer
 */
const tenantB: TenantContext = {
  tenantId: 'tenant-002-globex',
  tenantName: 'Globex Corporation',
  tenantDomain: 'globex.example.com',
  users: [
    {
      id: 'user-003-carol',
      email: 'carol@globex.example.com',
      name: 'Carol Admin',
      tenantId: 'tenant-002-globex',
      role: 'admin',
      idpDomain: 'globex.example.com',
      mfaEnabled: true,
    },
    {
      id: 'user-004-dave',
      email: 'dave@globex.example.com',
      name: 'Dave User',
      tenantId: 'tenant-002-globex',
      role: 'user',
      idpDomain: 'globex.example.com',
      mfaEnabled: false,
    },
  ],
  tasks: [
    {
      id: 'task-003-globex',
      title: 'Globex Merger Plans',
      description: 'Confidential M&A strategy',
      tenantId: 'tenant-002-globex',
      userId: 'user-003-carol',
      completed: false,
      createdAt: '2026-05-01T09:00:00Z',
    },
    {
      id: 'task-004-globex',
      title: 'Globex Customer Data',
      description: 'PII and customer information',
      tenantId: 'tenant-002-globex',
      userId: 'user-004-dave',
      completed: false,
      createdAt: '2026-05-02T11:45:00Z',
    },
  ],
};

/**
 * API helpers for test setup and validation
 */
export class TestApiClient {
  baseUrl: string;
  currentTenant: TenantContext | null = null;
  currentUser: TestUser | null = null;
  authToken: string | null = null;

  constructor(baseUrl: string = 'http://localhost:3000') {
    this.baseUrl = baseUrl;
  }

  /**
   * Simulate authentication for a user
   * PRECONDITION: Backend must expose /api/auth/login endpoint
   */
  async authenticateUser(user: TestUser): Promise<void> {
    this.currentUser = user;
    const tenantContext = [tenantA, tenantB].find(t => t.tenantId === user.tenantId);
    if (tenantContext) {
      this.currentTenant = tenantContext;
    }
    // Mock token with tenant_id claim
    this.authToken = this.mockJWT(user);
  }

  /**
   * Switch tenant context (for testing confused deputy attacks)
   */
  switchTenant(tenant: TenantContext): void {
    this.currentTenant = tenant;
  }

  /**
   * Mock JWT token with tenant claim
   * Format: header.payload.signature
   */
  private mockJWT(user: TestUser): string {
    const header = btoa(JSON.stringify({ alg: 'RS256', typ: 'JWT' }));
    const payload = btoa(
      JSON.stringify({
        sub: user.id,
        email: user.email,
        tenant_id: user.tenantId,
        iat: Math.floor(Date.now() / 1000),
        exp: Math.floor(Date.now() / 1000) + 3600,
      })
    );
    const signature = 'mock-signature-xyz';
    return `${header}.${payload}.${signature}`;
  }

  /**
   * GET /api/tasks
   * Should return only tasks for current tenant
   */
  async getTasks(): Promise<TestTask[]> {
    const response = await fetch(`${this.baseUrl}/api/tasks`, {
      headers: {
        Authorization: `Bearer ${this.authToken}`,
        'X-Tenant-ID': this.currentTenant?.tenantId || '',
      },
    });
    if (!response.ok) {
      throw new Error(`Failed to get tasks: ${response.status}`);
    }
    return response.json();
  }

  /**
   * GET /api/tasks/:id
   * Should validate tenant isolation before returning
   */
  async getTask(taskId: string): Promise<TestTask> {
    const response = await fetch(`${this.baseUrl}/api/tasks/${taskId}`, {
      headers: {
        Authorization: `Bearer ${this.authToken}`,
        'X-Tenant-ID': this.currentTenant?.tenantId || '',
      },
    });
    if (!response.ok) {
      throw new Error(`Failed to get task: ${response.status}`);
    }
    return response.json();
  }

  /**
   * POST /api/tasks
   * Should scope task to current user's tenant
   */
  async createTask(title: string, description: string): Promise<TestTask> {
    const response = await fetch(`${this.baseUrl}/api/tasks`, {
      method: 'POST',
      headers: {
        Authorization: `Bearer ${this.authToken}`,
        'X-Tenant-ID': this.currentTenant?.tenantId || '',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ title, description }),
    });
    if (!response.ok) {
      throw new Error(`Failed to create task: ${response.status}`);
    }
    return response.json();
  }

  /**
   * GET /api/users
   * Admin endpoint - should return only users from admin's tenant
   */
  async getUsers(): Promise<TestUser[]> {
    const response = await fetch(`${this.baseUrl}/api/users`, {
      headers: {
        Authorization: `Bearer ${this.authToken}`,
        'X-Tenant-ID': this.currentTenant?.tenantId || '',
      },
    });
    if (!response.ok) {
      throw new Error(`Failed to get users: ${response.status}`);
    }
    return response.json();
  }

  /**
   * Verify all tasks returned belong to current tenant
   */
  verifyTasksAreFromTenant(tasks: TestTask[], expectedTenantId: string): boolean {
    return tasks.every(task => task.tenantId === expectedTenantId);
  }

  /**
   * Verify all users returned belong to current tenant
   */
  verifyUsersAreFromTenant(users: TestUser[], expectedTenantId: string): boolean {
    return users.every(user => user.tenantId === expectedTenantId);
  }
}

/**
 * Playwright fixture that provides multi-tenant test context
 */
export const test = base.extend<{
  tenantA: TenantContext;
  tenantB: TenantContext;
  apiClient: TestApiClient;
}>({
  tenantA: async ({}, use) => {
    await use(tenantA);
  },

  tenantB: async ({}, use) => {
    await use(tenantB);
  },

  apiClient: async ({}, use) => {
    const client = new TestApiClient();
    await use(client);
  },
});

export { expect } from '@playwright/test';
