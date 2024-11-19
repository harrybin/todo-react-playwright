export interface Task {
  id: string;
  name: string;
  time: string;
  location: {
    latitude: number;
    longitude: number;
  };
  completed: boolean;
}
