import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.tsx";
import "./index.css";
import type { Task } from "./Task.ts";

const DATA: Task[] = [
  {
    id: "todo-iYhueLHTq-6wprHhsXYF6",
    name: "test",
    time: "2024-11-18T16:12:44.160Z",
    location: {
      latitude: 49.6370557,
      longitude: 6.9014314,
    },
    completed: false,
  },
];

const rootElement = document.getElementById("root");
if (rootElement) {
  ReactDOM.createRoot(rootElement).render(
    <React.StrictMode>
      <App tasks={DATA} />
    </React.StrictMode>
  );
} else {
  console.error("Failed to find the root element");
}
