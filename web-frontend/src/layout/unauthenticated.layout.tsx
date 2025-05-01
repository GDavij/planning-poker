import * as React from "react";
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import IconButton from "@mui/material/IconButton";
import Typography from "@mui/material/Typography";
import Menu from "@mui/material/Menu";
import MenuIcon from "@mui/icons-material/Menu";
import Container from "@mui/material/Container";
import Tooltip from "@mui/material/Tooltip";
import MenuItem from "@mui/material/MenuItem";
import AdbIcon from "@mui/icons-material/Adb";
import { AccountCircleRounded } from "@mui/icons-material";
import { Reference } from "../models/reference";
import { Link } from "@mui/material";
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
  return (
    <>
      <AppBar position="static" sx={{ backgroundColor: "#1976d2", height: 60 }}>
        <Container maxWidth="xl">
          <Toolbar disableGutters>
            <Typography
              variant="h6"
              noWrap
              component="div"
              sx={{
                flexGrow: 1,
                fontWeight: "bold",
                color: "white",
                textDecoration: "none",
              }}
            >
              Planning Poker
            </Typography>
          </Toolbar>
        </Container>
      </AppBar>

      <Box
        sx={{
          padding: "32px",
          margin: "16px auto",
          maxWidth: "1200px",
        }}
      >
        <Outlet />
      </Box>
    </>
  );
}
