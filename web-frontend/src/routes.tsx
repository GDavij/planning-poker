import { createBrowserRouter, Outlet } from "react-router";
import { UnauthenticatedLayout } from "./layout/unauthenticated.layout";
import { CssBaseline } from "@mui/material";
import { LandingPage } from "./pages/landing-page";
import { SignInPage } from "./pages/auth/signin/signin";
import { HomeDashboard } from "./pages/dashboard/home.dashboard";
import { MatchGamePage } from "./layout/match-connection.layout";
import { CreateMatchPage } from "./pages/matches/create-match.dashboard";
import { AuthenticatedLayout } from "./layout/authenticated.layout";
import { PartyPage } from "./pages/matches/party/party-page";
import { JoinMatchPage } from "./pages/matches/join/join-match.page";

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
        ],
      },
      {
        path: "/dashboard",
        Component: AuthenticatedLayout,
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
              {
                path: "party/:matchId",
                Component: PartyPage,
              },
            ],
          },
        ],
      },
    ],
  },
]);
