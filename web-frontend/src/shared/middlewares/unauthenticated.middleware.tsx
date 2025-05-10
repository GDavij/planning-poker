import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { Reference } from "../models/reference";
import { Outlet } from "react-router";

const pages: Reference[] = [
  {
    path: "/join",
    name: "Join a Game",
  },
  {
    path: "/pricing",
    name: "Pricing",
  },
];
const settings: Reference[] = [
  {
    path: "/signup",
    name: "Signup",
  },
  {
    path: "/signin",
    name: "Signin",
  },
];

export function UnauthenticatedLayout() {
  return <Outlet />;
}
