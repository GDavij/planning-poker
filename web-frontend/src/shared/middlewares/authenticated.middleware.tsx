import { Outlet, useNavigate } from "react-router";
import { useGetMe } from "../hooks/integrations/api/auth/get-me.integration";
import { CreateStoryFormModal } from "../../features/stories/create-story.modal.form";
import { JoinMatchModal } from "../../features/stories/join-match.modal.form";

export function AuthenticatedLayout() {
  const { me } = useGetMe();

  return (
    <>
      <Outlet />
      <CreateStoryFormModal />
      <JoinMatchModal />
    </>
  );
}
