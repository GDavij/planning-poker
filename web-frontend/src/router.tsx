import { createBrowserRouter, Outlet } from "react-router";
import { CssBaseline } from "@mui/material";
import { HomeDashboard } from "./pages/dashboard/home.dashboard";
import { MatchGamePage } from "./shared/middlewares/match-connection.middleware";
import { CreateMatchPage } from "./pages/matches/create-match.dashboard";
import { AuthenticatedLayout } from "./shared/middlewares/authenticated.middleware";
import { PartyPage } from "./pages/matches/party/party-page";
import { JoinMatchPage } from "./pages/matches/join/join-match.page";
import { UnauthenticatedLayout } from "./shared/middlewares/unauthenticated.middleware";
import { SignInPage } from "./pages/auth/sign-in/sign-in";
import { LandingPage } from "./pages/landing-page";
import { newAccountPage } from "./pages/auth/new-account/new-account";

const CssLayout = () => {
  return (
    <>
      <CssBaseline />
      <Outlet />
    </>
  );
};

export const router = createBrowserRouter([
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
            path: "new-account",
            Component: newAccountPage,
          },
        ],
      },
    ],
  },
]);
