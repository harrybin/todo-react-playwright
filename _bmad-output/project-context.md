---
project_name: 'todo-react'
user_name: 'Harry'
date: '2026-05-03'
sections_completed: ['technology_stack', 'critical_implementation_rules', 'code_patterns', 'testing_architecture']
patterns_discovered: 7
rules_documented: 12
last_updated: '2026-05-03'
---

# Project Context for AI Agents

_This file contains critical rules and patterns that AI agents must follow when implementing code in this project. Focus on unobvious details that agents might otherwise miss._

---

## Technology Stack & Versions

| Technology | Version | Purpose | Notes |
|-----------|---------|---------|-------|
| React | 19.1.0 | UI library | Functional components + hooks only |
| React DOM | 19.1.0 | DOM rendering | No SSR or classic JSX transform |
| TypeScript | 5.8.3 | Language | Strict type checking enabled |
| Vite | 6.3.5 | Build tool | Dev server runs on port 3000 |
| Material-UI (MUI) | 7.1.0 | Component library | Primary UI framework for all components |
| Emotion | 11.14.0 | CSS-in-JS | Styling engine for MUI and custom styles |
| Playwright | Latest | E2E Testing | Browser automation and test runner |
| ESLint | 9.26.0 | Linting | Strict ruleset with React plugins |
| nanoid | 5.1.5 | Utilities | Lightweight ID generation for tasks |

### Build & Dev Configuration
- **Module Type**: ESM (`"type": "module"` in package.json)
- **Dev Server Port**: 3000 (configured in vite.config.js)
- **Base URL**: http://localhost:3000/
- **Target**: ES2020 (via ESLint config)

---

## Critical Implementation Rules

### 1. Component Architecture

**RULE: Use functional components with hooks exclusively**
- No class components under any circumstances
- All state management via `useState`, `useEffect`, `useRef`
- Custom hooks must be typed with TypeScript generics: `function usePrevious<T>(value: T): T | null`

**RULE: Explicit React imports required**
```typescript
// ✅ CORRECT
import React from "react";
import { useState } from "react";

// ❌ WRONG
import { FC } from "react"; // Don't use FC type
```

**RULE: Props must use TypeScript interfaces**
```typescript
// ✅ CORRECT
interface FormProps {
  addTask: (name: string) => void;
}

function Form(props: FormProps) { ... }

// ❌ WRONG
function Form(props: { addTask: Function }) { ... }
```

### 2. TypeScript Configuration

**RULE: Strict typing throughout**
- All component props must be explicitly typed
- Event handlers must use React event types: `React.FormEvent`, `React.ChangeEvent<HTMLInputElement>`
- Generic types preferred for reusable utilities
- No `any` types allowed

**RULE: Type domain models in separate files**
- Example: `src/Task.ts` defines the `Task` interface used across components
- Import types as: `import type { Task } from "./Task";`

### 3. Component File Structure

**RULE: PascalCase for component filenames**
- Components: `Form.tsx`, `Todo.tsx`, `FilterButton.tsx`
- Utilities: `Task.ts`, `App.tsx`, `main.tsx`

**RULE: Single component per file**
- Each `.tsx` file exports one primary React component
- Helper functions/utilities can be colocated

### 4. Styling & UI Library

**RULE: Use Material-UI (MUI) for all UI components**
- Import from `@mui/material`: `import { Button, TextField, Stack } from "@mui/material";`
- Avoid custom HTML elements; use MUI equivalents for consistency
- Emotion handles all CSS-in-JS through MUI's theming

**RULE: Layout via Stack component**
- Use `<Stack>` for vertical/horizontal layouts
- Use `<Grid>` for grid-based layouts
- Stack props: `spacing`, `direction`, `alignItems`, `justifyContent`

**RULE: Form elements via MUI**
- `<TextField>` for text inputs (not native `<input>`)
- `<Button>` for all buttons (not native `<button>`)
- `<Checkbox>`, `<FormControlLabel>` for checkbox inputs

### 5. Event Handling

**RULE: Use typed React event handlers**
```typescript
// ✅ CORRECT
function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
  setName(event.target.value);
}

function handleSubmit(event: React.FormEvent) {
  event.preventDefault();
}

// ❌ WRONG
const handleChange = (e: any) => { ... }
```

**RULE: Handlers accept proper event object, not inline values**
- Extract values from `event.target.value` or `event.target.checked`
- Prevent default on form submissions with `event.preventDefault()`

### 6. Custom Hooks Pattern

**RULE: Generic custom hooks with full typing**
```typescript
// ✅ CORRECT
function usePrevious<T>(value: T): T | null {
  const ref = useRef<T | null>(null);
  useEffect(() => {
    ref.current = value;
  });
  return ref.current;
}

// Usage in component
const wasEditing = usePrevious(isEditing);
```

### 7. State Management

**RULE: Use React hooks for all state**
- `useState` for component state
- `useRef` for DOM references and stable values across renders
- `useEffect` for side effects (DOM focus, listeners, etc.)

**RULE: Update state with new objects/arrays, never mutate**
```typescript
// ✅ CORRECT
setTasks([...tasks, newTask]);
setTask({ ...task, name: newName });

// ❌ WRONG
tasks.push(newTask);
task.name = newName;
```

### 8. ID Generation

**RULE: Use nanoid for unique IDs**
```typescript
import { nanoid } from "nanoid";

const newTask = {
  id: nanoid(),
  name: taskName,
  // ...
};
```
- Never use Math.random() or timestamps for IDs
- nanoid produces URL-safe, collision-resistant IDs

### 9. Code Organization

**RULE: Component organization hierarchy**
```
src/
├── App.tsx              (Main container, state orchestration)
├── components/
│   ├── Form.tsx         (Input form - single responsibility)
│   ├── Todo.tsx         (Task item - rendering + editing)
│   └── FilterButton.tsx (Filter control)
├── Task.ts              (Domain types only - no logic)
└── main.tsx             (React entry point)
```

### 10. ESLint Compliance

**RULE: All ESLint rules must pass**
- Command: `npm run lint` must succeed with zero warnings
- React JSX first prop on new line for multiline JSX (configured in .eslintrc.cjs)
- React Hooks linting rules are enforced
- React Refresh compatible exports

### 11. Testing Architecture

**RULE: Use Playwright for E2E testing**
- All E2E tests use Playwright browser automation
- Test command: `npm run test` (headless)
- UI mode: `npm run test:ui` (interactive browser)
- Tests directory: `tests/` (currently empty, ready for implementation)

**RULE: No unit test framework configured yet**
- Focus on E2E Playwright tests only
- Integration tests via Playwright scenarios

### 12. Import Conventions

**RULE: Organize imports in order**
```typescript
// 1. React and third-party libraries
import { useState, useRef, useEffect } from "react";
import React from "react";

// 2. Third-party components/utilities
import { nanoid } from "nanoid";
import { Container, Button } from "@mui/material";

// 3. Local types (with `type` keyword)
import type { Task } from "./Task";

// 4. Local components
import Form from "./components/Form";
import Todo from "./components/Todo";
```

---

## Code Patterns Reference

### Pattern: Form Input with State
```typescript
const [name, setName] = useState("");

function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
  setName(event.target.value);
}

function handleSubmit(event: React.FormEvent) {
  event.preventDefault();
  // Process name
  setName("");
}
```

### Pattern: Task Editing with usePrevious
```typescript
const [isEditing, setEditing] = useState(false);
const wasEditing = usePrevious(isEditing);

useEffect(() => {
  if (wasEditing && !isEditing) {
    // Editing just finished
  }
}, [isEditing, wasEditing]);
```

### Pattern: Conditional Filtering
```typescript
const FILTER_MAP = {
  All: () => true,
  Active: (task: { completed: boolean }) => !task.completed,
  Completed: (task: { completed: boolean }) => task.completed,
};

const filteredTasks = tasks.filter(FILTER_MAP[filter]);
```

---

## Testing Architecture

- **Framework**: Playwright for E2E browser testing
- **Test Location**: `tests/` directory (currently empty)
- **Dev Command**: `npm run test:ui` for interactive testing
- **CI Command**: `npm run test` for headless testing
- **Configuration**: No playwright.config.ts currently (using defaults)

---

## Common Pitfalls to Avoid

❌ **DON'T:** Use class components or function component types (`React.FC`)
❌ **DON'T:** Mutate state directly (`tasks.push()`, `task.name =`)
❌ **DON'T:** Use `any` types; always be explicit with TypeScript
❌ **DON'T:** Use Math.random() or timestamps for IDs
❌ **DON'T:** Mix native HTML elements with MUI components in the same form
❌ **DON'T:** Forget `event.preventDefault()` on form submissions
❌ **DON'T:** Create untyped event handlers
❌ **DON'T:** Use Redux or complex state management (hooks only)

---

## Quick Reference Commands

```bash
# Development
npm run dev              # Start dev server at http://localhost:3000

# Build & Deployment
npm run build            # Build for production
npm run preview          # Preview production build locally
npm run deploy           # Deploy to GitHub Pages

# Code Quality
npm run lint             # Check ESLint (must pass with 0 warnings)

# Testing
npm run test             # Run Playwright E2E tests (headless)
npm run test:ui          # Run Playwright tests (interactive UI mode)
```

---

**Last Generated**: 2026-05-03 by Generate Project Context workflow
**Next Review**: After completing next feature cycle
