import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { Reference } from "../models/reference";
import { Link, Outlet, Routes } from "react-router";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Button,
  Card,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListSubheader,
  Paper,
  Stack,
} from "@mui/material";
import {
  Close,
  Engineering,
  Home,
  Info,
  Login,
  Menu,
  PersonAdd,
} from "@mui/icons-material";
import { useState } from "react";
import { useMediaQuery } from "@uidotdev/usehooks";
import { RoutesDef } from "../consts/routesDef";

export function UnauthenticatedLayout() {
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);

  const toggleDrawer = () => setIsDrawerOpen((v) => !v);

  const isMobile = useMediaQuery("only screen and (max-width : 768px)");

  return (
    <>
      <Card
        sx={{
          borderRadius: 0,
          height: 80,
          display: "flex",
          alignItems: "center",
          px: 1,
          position: "sticky",
          mb: 4,
        }}
      >
        <IconButton onClick={toggleDrawer}>
          <Menu />
        </IconButton>
      </Card>
      <Outlet />
      <Drawer open={isDrawerOpen} onClose={toggleDrawer}>
        <Box
          width={isMobile ? "100vw" : 300}
          height={80}
          display={"flex"}
          flexDirection={"column"}
        >
          <Box
            height={80}
            display={"flex"}
            justifyContent={"center"}
            alignItems={"space-between"}
            px={2}
          >
            <Box
              display={"flex"}
              justifyContent={"center"}
              alignItems={"center"}
              width={"100%"}
            >
              <Typography variant="h5"> Planning Poker </Typography>
            </Box>
            <Box display={"flex"} alignItems={"center"}>
              <IconButton
                sx={{ height: "fit-content", width: "fit-content" }}
                onClick={toggleDrawer}
              >
                <Close />
              </IconButton>
            </Box>
          </Box>
        </Box>
        <Divider sx={{ marginX: 0.5 }} />

        <List>
          <List>
            <ListSubheader> About us </ListSubheader>
            <ListItemButton
              component={Link}
              to={RoutesDef.Home}
              onClick={toggleDrawer}
            >
              <Home />
              <Typography variant="button"> Home </Typography>
            </ListItemButton>

            <ListItemButton
              component={Link}
              to={RoutesDef.About}
              onClick={toggleDrawer}
            >
              <Info />
              <Typography variant="button"> About this Project </Typography>
            </ListItemButton>

            <ListItemButton
              component={Link}
              to={RoutesDef.Engineering}
              onClick={toggleDrawer}
            >
              <Engineering />
              <Typography variant="button"> Project Engineering </Typography>
            </ListItemButton>
          </List>
          <List>
            <ListSubheader> Login </ListSubheader>
            <ListItemButton
              component={Link}
              to={RoutesDef.SignIn}
              onClick={toggleDrawer}
            >
              <Login />
              <Typography variant="button"> Sign in </Typography>
            </ListItemButton>
            <ListItemButton
              component={Link}
              to={RoutesDef.CreateAccount}
              onClick={toggleDrawer}
            >
              <PersonAdd />
              <Typography variant="button"> New Account </Typography>
            </ListItemButton>
          </List>
        </List>
      </Drawer>
    </>
  );
}
