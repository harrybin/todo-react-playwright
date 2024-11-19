import { useState, useRef, useEffect } from "react";
import Form from "./components/Form";
import FilterButton from "./components/FilterButton";
import Todo from "./components/Todo";
import { nanoid } from "nanoid";
import React from "react";
import type { Task } from "./Task";

function usePrevious(value: number) {
  const ref = useRef<number | null>(null);
  useEffect(() => {
    ref.current = value;
  });
  return ref.current;
}

const FILTER_MAP = {
  All: () => true,
  Active: (task: { completed: Task }) => !task.completed,
  Completed: (task: { completed: Task }) => task.completed,
};

const FILTER_NAMES = Object.keys(FILTER_MAP);

function App(props: { tasks: Task[] }) {
  const [tasks, setTasks] = useState<Task[]>(props.tasks);
  const [filter, setFilter] = useState("All");

  function toggleTaskCompleted(task: Task) {
    const updatedTasks: Task[] = tasks.map((itask: Task) => {
      // if this task has the same ID as the edited task
      if (task.id === itask.id) {
        // use object spread to make a new obkect
        // whose `completed` prop has been inverted
        return { ...itask, completed: !itask.completed };
      }
      return task;
    });
    setTasks(updatedTasks);
  }

  function deleteTask(task: Task) {
    const remainingTasks = tasks.filter((task: Task) => task.id !== task.id);
    setTasks(remainingTasks);
  }

  function editTask(task: Task) {
    const editedTaskList = tasks.map((itask: Task) => {
      // if this task has the same ID as the edited task
      if (task.id === itask.id) {
        // Copy the task and update its name
        return { ...itask, name: task.name } as Task;
      }
      // Return the original task if it's not the edited task
      return task as Task;
    });
    setTasks(editedTaskList);
  }

  const taskList = tasks
    ?.filter(FILTER_MAP[filter])
    .map((task: Task) => (
      <Todo
        task={task}
        key={task.id}
        toggleTaskCompleted={toggleTaskCompleted}
        deleteTask={deleteTask}
        editTask={editTask}
      />
    ));

  const filterList = FILTER_NAMES.map((name) => (
    <FilterButton
      key={name}
      name={name}
      isPressed={name === filter}
      setFilter={setFilter}
    />
  ));

  function addTask(name: string) {
    navigator.geolocation.getCurrentPosition((position) => {
      const { latitude, longitude } = position.coords;
      const newTask = {
        id: "todo-" + nanoid(),
        name: name,
        time: new Date().toISOString(),
        location: { latitude, longitude },
        completed: false,
      } as Task;
      setTasks([...tasks, newTask]);
    });
  }

  const tasksNoun = taskList.length !== 1 ? "tasks" : "task";
  const headingText = `${taskList.length} ${tasksNoun} remaining`;

  const listHeadingRef = useRef<HTMLHeadingElement>(null);
  const prevTaskLength = usePrevious(tasks.length) ?? 0;

  useEffect(() => {
    if (tasks.length < prevTaskLength) {
      listHeadingRef.current?.focus();
    }
  }, [tasks.length, prevTaskLength]);

  return (
    // TODO: add Xebia log
    // load data from json file (fetch from server)
    <div className="todoapp stack-large">
      <h1>TodoMatic</h1>
      <Form addTask={addTask} />
      <div className="filters btn-group stack-exception">{filterList}</div>
      <h2 id="list-heading" tabIndex={-1} ref={listHeadingRef}>
        {headingText}
      </h2>
      <ul
        aria-labelledby="list-heading"
        className="todo-list stack-large stack-exception"
        role="list"
      >
        {taskList}
      </ul>
    </div>
  );
}

export default App;
