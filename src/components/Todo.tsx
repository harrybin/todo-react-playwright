import { useEffect, useRef, useState } from "react";
import React from "react";
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

  // NOTE: As written, this function has a bug: it doesn't prevent the user
  // from submitting an empty form. This is left as an exercise for developers
  // working through MDN's React tutorial.
  function handleSubmit(event: { preventDefault: () => void }) {
    event.preventDefault();
    props.editTask(props.task);
    setNewName("");
    setEditing(false);
  }

  const editingTemplate = (
    <form className="stack-small" onSubmit={handleSubmit}>
      <div className="form-group">
        <label className="todo-label" htmlFor={props.task.id}>
          New name for {props.task.name}
        </label>
        <input
          id={props.task.id}
          className="todo-text"
          type="text"
          value={newName}
          onChange={handleChange}
          ref={editFieldRef}
        />
      </div>
      <div className="btn-group">
        <button
          type="button"
          className="btn todo-cancel"
          onClick={() => setEditing(false)}
        >
          Cancel
          <span className="visually-hidden">renaming {props.task.name}</span>
        </button>
        <button type="submit" className="btn btn__primary todo-edit">
          Save
          <span className="visually-hidden">
            new name for {props.task.name}
          </span>
        </button>
      </div>
    </form>
  );

  const viewTemplate = (
    <div className="stack-small">
      <div className="c-cb">
        <input
          id={props.task.id}
          type="checkbox"
          defaultChecked={props.task.completed}
          onChange={() => props.toggleTaskCompleted(props.task)}
        />
        <label className="todo-label" htmlFor={props.task.id}>
          {`${props.task.name}  -  (${props.task.time})  -  [${props.task.location?.latitude}, ${props.task.location?.longitude}]`}
        </label>
      </div>
      <div className="btn-group">
        <button
          type="button"
          className="btn"
          onClick={() => {
            setEditing(true);
          }}
          ref={editButtonRef}
        >
          Edit <span className="visually-hidden">{props.task.name}</span>
        </button>
        <button
          type="button"
          className="btn btn__danger"
          onClick={() => props.deleteTask(props.task)}
        >
          Delete <span className="visually-hidden">{props.task.name}</span>
        </button>
      </div>
    </div>
  );

  useEffect(() => {
    if (!wasEditing && isEditing) {
      editFieldRef.current?.focus();
    } else if (wasEditing && !isEditing) {
      editButtonRef.current?.focus();
    }
  }, [wasEditing, isEditing]);

  return <li className="todo">{isEditing ? editingTemplate : viewTemplate}</li>;
}

export default Todo;
