import { useState } from "react";
import React from "react";
import { TextField, Button, Box, Typography, Stack } from "@mui/material";

interface FormProps {
  addTask: (name: string) => void;
}

function Form(props: FormProps) {
  const [name, setName] = useState("");

  function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    props.addTask(name);
    setName("");
  }

  function handleChange(event: React.ChangeEvent<HTMLInputElement>) {
    setName(event.target.value);
  }

  return (
    <Stack component="form" onSubmit={handleSubmit} spacing={2}>
      <Typography variant="h2">
        <label htmlFor="new-todo-input">What needs to be done?</label>
      </Typography>
      <TextField
        id="new-todo-input"
        name="text"
        autoComplete="off"
        value={name}
        onChange={handleChange}
        fullWidth
      />
      <Button
        type="submit"
        id="myUniqueID"
        variant="contained"
        color="primary"
        fullWidth
      >
        Add
      </Button>
    </Stack>
  );
}

export default Form;
