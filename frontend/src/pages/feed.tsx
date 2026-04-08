import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CommentsModal from "../components/CommentsModal";

type UserInfo = {
    id: number;
    username: string;
    profilePictureUrl?: string;
};

type FeedPost = {
    id: number;
    user: UserInfo;
    photoUrl: string;
    caption?: string;
    createdAt: string;
    updatedAt: string;
    likesCount: number;
    commentsCount: number;
};

type FeedResponse = {
    posts: FeedPost[];
};

export default function Feed() {
    const [posts, setPosts] = useState<FeedPost[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [selectedPostId, setSelectedPostId] = useState<number | null>(null);
    const [userId, setUserId] = useState<number | null>(null);
    const navigate = useNavigate();
    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

    useEffect(() => {
        fetchUserInfo();
        fetchFeedPosts();
    }, []);

    const fetchUserInfo = () => {
        try {
            const storedId = localStorage.getItem("id");
            if (storedId) {
                setUserId(parseInt(storedId, 10));
                console.log("User ID from localStorage:", storedId);
            } else {
                console.warn("No user ID found in localStorage");
            }
        } catch (err) {
            console.error("Failed to get user info:", err);
        }
    };

    const fetchFeedPosts = async () => {
        setLoading(true);
        setError(null);

        try {
            const res = await fetch(`${apiBaseUrl}/api/post/feed`, {
                method: "GET",
                credentials: "include",
            });

            if (!res.ok) {
                throw new Error(`Failed to fetch feed (${res.status})`);
            }

            const data: FeedResponse = await res.json();
            setPosts(data.posts);
        } catch (err) {
            console.error(err);
            setError("Failed to load feed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <div style={{ marginBottom: "20px" }}>
                <h1>Feed</h1>
                <button onClick={() => navigate("/dashboard")} style={{ marginTop: "10px" }}>
                    Go to Dashboard
                </button>
            </div>

            {loading && <p>Loading feed...</p>}
            {error && <p style={{ color: "red" }}>{error}</p>}

            <div>
                {posts.length === 0 && !loading && <p>No posts available.</p>}
                {posts.map((post) => (
                    <div key={post.id} style={{ borderBottom: "1px solid #ccc", padding: "15px 0" }}>
                        <div style={{ marginBottom: "10px" }}>
                            <strong>{post.user.username}</strong>
                            {post.user.profilePictureUrl && (
                                <img
                                    src={`${apiBaseUrl}${post.user.profilePictureUrl}`}
                                    alt={post.user.username}
                                    style={{ width: "40px", height: "40px", borderRadius: "50%", marginLeft: "10px" }}
                                />
                            )}
                        </div>
                        <img
                            src={`${apiBaseUrl}${post.photoUrl}`}
                            alt="Post"
                            style={{ maxWidth: "100%", height: "auto", marginBottom: "10px" }}
                        />
                        {post.caption && <p>{post.caption}</p>}
                        <div style={{ fontSize: "0.9em", color: "#666" }}>
                            <span>{post.likesCount} likes</span>
                            <span style={{ marginLeft: "15px" }}>{post.commentsCount} comments</span>
                            <span style={{ marginLeft: "15px" }}>{new Date(post.createdAt).toLocaleDateString()}</span>
                        </div>
                        <button
                            onClick={() => setSelectedPostId(post.id)}
                            style={{
                                marginTop: "10px",
                                padding: "8px 16px",
                                backgroundColor: "#007bff",
                                color: "white",
                                border: "none",
                                borderRadius: "4px",
                                cursor: "pointer",
                                fontSize: "0.9em",
                            }}
                        >
                            View Comments
                        </button>
                    </div>
                ))}
            </div>
            <CommentsModal
                postId={selectedPostId || 0}
                isOpen={selectedPostId !== null}
                onClose={() => setSelectedPostId(null)}
                userId={userId}
                apiBaseUrl={apiBaseUrl}
            />
        </div>
    );
}