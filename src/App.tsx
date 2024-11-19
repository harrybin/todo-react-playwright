import { useState, useRef, useEffect } from "react";
import Form from "./components/Form";
import FilterButton from "./components/FilterButton";
import Todo from "./components/Todo";
import { nanoid } from "nanoid";
import React from "react";
import {
  Container,
  Typography,
  Box,
  List,
  Stack,
  Grid2,
  Avatar,
  Button,
  Divider,
} from "@mui/material";
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
      if (task.id === itask.id) {
        return { ...itask, completed: !itask.completed };
      }
      return task;
    });
    setTasks(updatedTasks);
  }

  function deleteTask(task: Task) {
    const remainingTasks = tasks.filter((itask: Task) => itask.id !== task.id);
    setTasks(remainingTasks);
  }

  function editTask(task: Task) {
    const editedTaskList = tasks.map((itask: Task) => {
      if (task.id === itask.id) {
        return { ...itask, name: task.name } as Task;
      }
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

  const loadRemoteTasks = async () => {
    const response = await fetch("remoteTasks.json");
    const data = (await response.json()) as Task[];
    setTasks(data);
  };

  return (
    <Grid2 spacing={4} margin={4} maxWidth={900}>
      <img alt="Site Logo" src="getsitelogo.png" width="150" height="50" />
      <Stack spacing={2} marginLeft={20}>
        <Typography variant="h2">TodoMatic</Typography>
        <Form addTask={addTask} />
        <Box paddingBlock={2}>
          <Divider />
        </Box>
        <Button variant="contained" onClick={loadRemoteTasks}>
          <Typography>Load remote tasks</Typography>
        </Button>
        <Stack direction="row" spacing={2}>
          {filterList}
        </Stack>
        <Typography
          variant="h3"
          id="list-heading"
          tabIndex={-1}
          ref={listHeadingRef}
        >
          {headingText}
        </Typography>
        <List aria-labelledby="list-heading" role="list">
          {taskList}
        </List>
      </Stack>
    </Grid2>
  );
}

export default App;
