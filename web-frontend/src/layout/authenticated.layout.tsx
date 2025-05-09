import { Outlet, useNavigate } from "react-router";
import { CreateStoryFormModal } from "../forms/stories/create-story.modal.form";
import { useEffect } from "react";
import { useAuth } from "../stores/auth-store";
import { getCurrentAccount } from "../services/auth.service";
import { JoinMatchModal } from "../forms/stories/join-match.modal.form";
import { Box } from "@mui/material";

export function AuthenticatedLayout() {
  const { setAccountIdAs } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    getCurrentAccount()
      .then((account) => {
        setAccountIdAs(account.accountId);
      })
      .catch(() => navigate("/sign-in"));
  }, []);

  return (
    <>
      <Outlet />
      <CreateStoryFormModal />
      <JoinMatchModal />
    </>
  );
}
