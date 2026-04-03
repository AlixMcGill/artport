 import { Navigate, useNavigate } from "react-router-dom";
import ProfileHeader from "../components/profile/profileHeader";
import ProfileUserPosts from "../components/profile/ProfileUserPosts";

export default function Dashboard() {
    const navigate = useNavigate();

    return (
        <div>
            <h2>Profile</h2>
            <button onClick={() => navigate("/feed")} style={{ marginBottom: "20px" }}>
                Go to Feed
            </button>
            <ProfileHeader />
            <ProfileUserPosts />
        </div>
    );
}
