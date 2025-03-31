import { createBrowserRouter, Outlet } from "react-router";
import { UnauthenticatedLayout } from "./layout/unauthenticated.layout";
import { CssBaseline } from "@mui/material";
import { LandingPage } from "./pages/landing-page";
import { SignInPage } from "./pages/auth/signin/signin";
import { HomeDashboard } from "./pages/dashboard/home.dashboard";
import { MatchGamePage } from "./pages/matches/match-game";

const CssLayout = () => {
  return (
    <>
      <CssBaseline />
      <Outlet />
    </>
  );
};

export const routes = createBrowserRouter([
  {
    path: "",
    Component: CssLayout,
    children: [
      {
        path: "/",
        Component: UnauthenticatedLayout,
        children: [
          {
            path: "",
            Component: LandingPage,
          },
          {
            path: "sign-in",
            Component: SignInPage,
          },
          {
            path: "dashboard",
            Component: HomeDashboard,
          },
          {
            path: "match/:matchId",
            Component: MatchGamePage,
          },
        ],
      },
    ],
  },
]);
