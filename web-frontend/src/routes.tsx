import { createBrowserRouter, Outlet } from "react-router";
import { CssBaseline } from "@mui/material";
import { HomeDashboard } from "./pages/dashboard/home.dashboard";
import { MatchGamePage } from "./shared/middlewares/match-connection.middleware";
import { CreateMatchPage } from "./pages/matches/create-match.dashboard";
import { AuthenticatedLayout } from "./shared/middlewares/authenticated.middleware";
import { PartyPage } from "./pages/matches/party/party-page";
import { JoinMatchPage } from "./pages/matches/join/join-match.page";
import { UnauthenticatedLayout } from "./shared/middlewares/unauthenticated.middleware";
import { SignInPage } from "./pages/auth/signin/signin";

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
            Component: SignInPage,
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
