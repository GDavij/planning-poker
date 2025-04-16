import { createBrowserRouter, Outlet } from "react-router";
import { UnauthenticatedLayout } from "./layout/unauthenticated.layout";
import { CssBaseline } from "@mui/material";
import { LandingPage } from "./pages/landing-page";
import { SignInPage } from "./pages/auth/signin/signin";
import { HomeDashboard } from "./pages/dashboard/home.dashboard";
import { MatchGamePage } from "./pages/matches/match-connection-page";
import { MatchPage } from "./pages/matches/match";
import { CreateMatchPage } from "./pages/dashboard/matches/create-match.dashboard";
import { JoinMatchPage } from "./pages/matches/join-match.page";

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
            children: [
              {
                path: "",
                Component: HomeDashboard,
              },
              {
                path: "matches",
                Component: MatchGamePage,
                children: [
                  {
                    path: "new",
                    Component: CreateMatchPage,
                  },
                  {
                    path: "join/:matchId",
                    Component: JoinMatchPage,
                  },
                ],
              },
            ],
          },

          {
            path: "match/:matchId",
            Component: MatchGamePage,
            children: [
              {
                path: "",
                Component: MatchPage,
              },
            ],
          },
        ],
      },
    ],
  },
]);
