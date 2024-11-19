import React from "react";
import { Button } from "@mui/material";

interface FilterButtonProps {
  name: string;
  isPressed: boolean;
  setFilter: (name: string) => void;
}

function FilterButton(props: FilterButtonProps) {
  return (
    <Button
      data-testid={`testID-${props.name}`}
      aria-pressed={props.isPressed}
      onClick={() => props.setFilter(props.name)}
      variant={props.isPressed ? "contained" : "outlined"}
      color="primary"
    >
      <span>Show </span>
      <span>{props.name}</span>
      <span> tasks</span>
    </Button>
  );
}

export default FilterButton;
