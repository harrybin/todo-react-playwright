import { useState } from "react";
import React from "react";

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
    <form onSubmit={handleSubmit}>
      <h2 className="label-wrapper">
        <label htmlFor="new-todo-input" className="label__lg">
          What needs to be done?
        </label>
      </h2>

      <input
        type="text"
        id="new-todo-input"
        className="input input__lg"
        name="text"
        autoComplete="off"
        value={name}
        onChange={handleChange}
      />
      <button
        type="submit"
        id="myUniqueID"
        className="btn btn__primary btn__lg"
      >
        Add
      </button>
    </form>
  );
}

export default Form;
