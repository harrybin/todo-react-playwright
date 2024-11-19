import { useEffect, useRef, useState } from "react";
import React from "react";
import {
  TextField,
  Button,
  Box,
  Typography,
  Checkbox,
  ListItem,
  FormControlLabel,
  Stack,
} from "@mui/material";
import type { Task } from "../Task";

function usePrevious<T>(value: T): T | null {
  const ref = useRef<T | null>(null);
  useEffect(() => {
    ref.current = value;
  });
  return ref.current;
}

function Todo(props: {
  task: Task;
  editTask: (task: Task) => void;
  toggleTaskCompleted: (task: Task) => void;
  deleteTask: (task: Task) => void;
}) {
  const [isEditing, setEditing] = useState(false);
  const [newName, setNewName] = useState("");

  const editFieldRef = useRef<HTMLInputElement>(null);
  const editButtonRef = useRef<HTMLButtonElement>(null);

  const wasEditing = usePrevious(isEditing);

  function handleChange(event: {
    target: { value: React.SetStateAction<string> };
  }) {
    setNewName(event.target.value);
  }

  function handleSubmit(event: { preventDefault: () => void }) {
    event.preventDefault();
    props.editTask({ ...props.task, name: newName });
    setNewName("");
    setEditing(false);
  }

  const editingTemplate = (
    <Box component="form" onSubmit={handleSubmit}>
      <div>
        <Typography variant="body1">New name for {props.task.name}</Typography>
        <TextField
          id={props.task.id}
          type="text"
          value={newName}
          onChange={handleChange}
          inputRef={editFieldRef}
          fullWidth
        />
      </div>
      <div>
        <Button
          type="button"
          onClick={() => setEditing(false)}
          variant="outlined"
          color="secondary"
        >
          Cancel
          <span className="visually-hidden">renaming {props.task.name}</span>
        </Button>
        <Button type="submit" variant="contained" color="primary">
          Save
          <span className="visually-hidden">
            new name for {props.task.name}
          </span>
        </Button>
      </div>
    </Box>
  );

  const viewTemplate = (
    <Stack spacing={2}>
      <Box>
        <FormControlLabel
          control={
            <Checkbox
              id={props.task.id}
              checked={props.task.completed}
              onChange={() => props.toggleTaskCompleted(props.task)}
            />
          }
          label={
            <Box>
              <Typography variant="h4">{props.task.name}</Typography>
              <Typography variant="h5">{`(${props.task.time})  -  [${props.task.location?.latitude}, ${props.task.location?.longitude}]`}</Typography>
            </Box>
          }
        />
      </Box>
      <Stack direction="row" spacing={2}>
        <Button
          type="button"
          onClick={() => {
            setEditing(true);
          }}
          ref={editButtonRef}
          variant="outlined"
          color="primary"
        >
          Edit <span className="visually-hidden">{props.task.name}</span>
        </Button>
        <Button
          type="button"
          onClick={() => props.deleteTask(props.task)}
          variant="contained"
          color="secondary"
        >
          Delete <span className="visually-hidden">{props.task.name}</span>
        </Button>
      </Stack>
    </Stack>
  );

  useEffect(() => {
    if (!wasEditing && isEditing) {
      editFieldRef.current?.focus();
    } else if (wasEditing && !isEditing) {
      editButtonRef.current?.focus();
    }
  }, [wasEditing, isEditing]);

  return <ListItem>{isEditing ? editingTemplate : viewTemplate}</ListItem>;
}

export default Todo;
