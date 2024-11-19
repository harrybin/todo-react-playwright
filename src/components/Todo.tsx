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
  Paper,
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
    <Paper>
      <Stack component="form" onSubmit={handleSubmit} margin={2} spacing={2}>
        <Typography variant="h5">{`New name for "${props.task.name}"`}</Typography>
        <TextField
          id={props.task.id}
          type="text"
          value={newName}
          onChange={handleChange}
          inputRef={editFieldRef}
          fullWidth
        />
        <Stack direction="row" spacing={2}>
          <Button
            type="button"
            onClick={() => setEditing(false)}
            variant="outlined"
            color="secondary"
          >
            Cancel
          </Button>
          <Button type="submit" variant="contained" color="primary">
            Save
          </Button>
        </Stack>
      </Stack>
    </Paper>
  );

  const viewTemplate = (
    <Paper>
      <Stack spacing={2} margin={2}>
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
            Edit
          </Button>
          <Button
            type="button"
            onClick={() => props.deleteTask(props.task)}
            variant="contained"
            color="secondary"
          >
            Delete
          </Button>
        </Stack>
      </Stack>
    </Paper>
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
