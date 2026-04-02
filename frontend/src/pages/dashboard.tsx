 import { Navigate } from "react-router-dom";
import ProfileHeader from "../components/profile/profileHeader";
import ProfileUserPosts from "../components/profile/ProfileUserPosts";

export default function Dashboard() {
    return (
        <div>
            <h2>Profile</h2>
            <ProfileHeader />
            <ProfileUserPosts />
        </div>
    );
}
