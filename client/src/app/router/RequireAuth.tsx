import { Navigate, Outlet, useLocation } from "react-router";
import { useAccount } from "../../lib/hooks/useAccount";
import { Typography } from "@mui/material";


export default function RequireAuth() {
    const { currentUser, loadingUserInfo } = useAccount();
    const location = useLocation();
    if (loadingUserInfo) return <Typography>Loading...</Typography>
    // console.log(currentUser);
    if (!currentUser) {
        // console.log(currentUser);
        return <Navigate to='/login' state={{ from: location }} />
    }

    return (
        <Outlet />
    )
}
