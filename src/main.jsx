import React from "react";
import ReactDOM from "react-dom/client";
import App from "./App.jsx";
import "./index.css";

// eslint-disable-next-line react-refresh/only-export-components
const DATA = [
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

ReactDOM.createRoot(document.getElementById("root")).render(
  <React.StrictMode>
    <App tasks={DATA} />
  </React.StrictMode>
);
