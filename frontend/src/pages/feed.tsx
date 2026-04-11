import { useCallback, useEffect, useRef, useState } from "react";
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

type SortOption = "latest" | "trending";

export default function Feed() {
    const [posts, setPosts] = useState<FeedPost[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [paginationLoading, setPaginationLoading] = useState(false);
    const [selectedPostId, setSelectedPostId] = useState<number | null>(null);
    const [userId, setUserId] = useState<number | null>(null);
    const [sort, setSort] = useState<SortOption>("latest");
    const [page, setPage] = useState(1);
    const [hasMore, setHasMore] = useState(true);
    const observerTarget = useRef<HTMLDivElement>(null);
    const navigate = useNavigate();
    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

    useEffect(() => {
        fetchUserInfo();
    }, []);

    const fetchUserInfo = () => {
        try {
            const storedId = localStorage.getItem("id");
            if (storedId) {
                setUserId(parseInt(storedId, 10));
            } else {
                console.warn("No user ID found in localStorage");
            }
        } catch (err) {
            console.error("Failed to get user info:", err);
        }
    };

    const fetchFeedPosts = useCallback(
        async (pageNum: number, sortType: SortOption, isInitial: boolean = false) => {
            const setLoadingState = isInitial ? setLoading : setPaginationLoading;
            setLoadingState(true);
            setError(null);

            try {
                const params = new URLSearchParams();
                params.append("page", pageNum.toString());
                params.append("pageSize", "20");
                if (sortType === "trending") {
                    params.append("sort", "trending");
                }

                const res = await fetch(`${apiBaseUrl}/api/post/feed?${params.toString()}`, {
                    method: "GET",
                    credentials: "include",
                });

                if (!res.ok) {
                    throw new Error(`Failed to fetch feed (${res.status})`);
                }

                const data: FeedResponse = await res.json();

                if (isInitial) {
                    setPosts(data.posts);
                } else {
                    setPosts((prevPosts) => [...prevPosts, ...data.posts]);
                }

                setHasMore(data.posts.length === 20);
            } catch (err) {
                console.error(err);
                setError("Failed to load feed");
            } finally {
                setLoadingState(false);
            }
        },
        [apiBaseUrl]
    );

    // Reset pagination and fetch when sort changes
    useEffect(() => {
        setPage(1);
        fetchFeedPosts(1, sort, true);
    }, [sort, fetchFeedPosts]);

    // Initial fetch on component mount
    useEffect(() => {
        if (page === 1) {
            fetchFeedPosts(1, sort, true);
        }
    }, []);

    // Intersection Observer for infinite scroll
    useEffect(() => {
        const observer = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting && hasMore && !paginationLoading && !loading) {
                    const nextPage = page + 1;
                    setPage(nextPage);
                    fetchFeedPosts(nextPage, sort, false);
                }
            },
            { threshold: 0.1 }
        );

        if (observerTarget.current) {
            observer.observe(observerTarget.current);
        }

        return () => {
            observer.disconnect();
        };
    }, [page, hasMore, paginationLoading, loading, sort, fetchFeedPosts]);

    const handleLike = useCallback(
        async (postId: number) => {
            try {
                const res = await fetch(`${apiBaseUrl}/api/like/${postId}`, {
                    method: "POST",
                    credentials: "include",
                });

                if (!res.ok) {
                    const errorData = await res.text();
                    throw new Error(errorData || `Failed to like post (${res.status})`);
                }

                // Update the post's like count optimistically
                setPosts((prevPosts) =>
                    prevPosts.map((post) =>
                        post.id === postId ? { ...post, likesCount: post.likesCount + 1 } : post
                    )
                );
            } catch (err) {
                console.error("Failed to like post:", err);
                alert("Failed to like post. Please try again.");
            }
        },
        [apiBaseUrl]
    );

    const handleSortChange = (newSort: SortOption) => {
        setSort(newSort);
    };

    return (
        <div style={{ maxWidth: "800px", margin: "0 auto", padding: "20px" }}>
            <div style={{ marginBottom: "30px" }}>
                <h1>Feed</h1>

                {/* Sort Toggle */}
                <div style={{ marginBottom: "20px", display: "flex", gap: "10px", alignItems: "center" }}>
                    <span style={{ fontWeight: "600" }}>Sort by:</span>
                    <button
                        onClick={() => handleSortChange("latest")}
                        style={{
                            padding: "8px 16px",
                            backgroundColor: sort === "latest" ? "#007bff" : "#e9ecef",
                            color: sort === "latest" ? "white" : "#333",
                            border: "none",
                            borderRadius: "4px",
                            cursor: "pointer",
                            fontWeight: sort === "latest" ? "600" : "400",
                        }}
                    >
                        Latest
                    </button>
                    <button
                        onClick={() => handleSortChange("trending")}
                        style={{
                            padding: "8px 16px",
                            backgroundColor: sort === "trending" ? "#007bff" : "#e9ecef",
                            color: sort === "trending" ? "white" : "#333",
                            border: "none",
                            borderRadius: "4px",
                            cursor: "pointer",
                            fontWeight: sort === "trending" ? "600" : "400",
                        }}
                    >
                        Trending
                    </button>
                </div>

                <button
                    onClick={() => navigate("/dashboard")}
                    style={{
                        padding: "8px 16px",
                        backgroundColor: "#6c757d",
                        color: "white",
                        border: "none",
                        borderRadius: "4px",
                        cursor: "pointer",
                    }}
                >
                    Go to Dashboard
                </button>
            </div>

            {loading && <p>Loading feed...</p>}
            {error && <p style={{ color: "red" }}>{error}</p>}

            <div>
                {posts.length === 0 && !loading && <p>No posts available.</p>}
                {posts.map((post) => (
                    <div key={post.id} style={{ borderBottom: "1px solid #ccc", padding: "15px 0" }}>
                        <div style={{ marginBottom: "10px", display: "flex", alignItems: "center", gap: "10px" }}>
                            {post.user.profilePictureUrl && (
                                <img
                                    src={`${apiBaseUrl}${post.user.profilePictureUrl}`}
                                    alt={post.user.username}
                                    style={{ width: "40px", height: "40px", borderRadius: "50%" }}
                                />
                            )}
                            <strong>{post.user.username}</strong>
                        </div>

                        <img
                            src={`${apiBaseUrl}${post.photoUrl}`}
                            alt="Post"
                            style={{ maxWidth: "100%", height: "auto", marginBottom: "10px", borderRadius: "4px" }}
                        />

                        {post.caption && <p style={{ marginBottom: "10px" }}>{post.caption}</p>}

                        <div style={{ fontSize: "0.9em", color: "#666", marginBottom: "10px" }}>
                            <span>{post.likesCount} likes</span>
                            <span style={{ marginLeft: "15px" }}>{post.commentsCount} comments</span>
                            <span style={{ marginLeft: "15px" }}>{new Date(post.createdAt).toLocaleDateString()}</span>
                        </div>

                        <div style={{ display: "flex", gap: "10px" }}>
                            <button
                                onClick={() => handleLike(post.id)}
                                style={{
                                    padding: "8px 16px",
                                    backgroundColor: "#dc3545",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    cursor: "pointer",
                                    fontSize: "0.9em",
                                }}
                            >
                                ❤️ Like
                            </button>
                            <button
                                onClick={() => setSelectedPostId(post.id)}
                                style={{
                                    padding: "8px 16px",
                                    backgroundColor: "#007bff",
                                    color: "white",
                                    border: "none",
                                    borderRadius: "4px",
                                    cursor: "pointer",
                                    fontSize: "0.9em",
                                }}
                            >
                                💬 View Comments
                            </button>
                        </div>
                    </div>
                ))}

                {/* Intersection observer target for infinite scroll */}
                <div
                    ref={observerTarget}
                    style={{
                        padding: "20px",
                        textAlign: "center",
                        visibility: hasMore ? "visible" : "hidden",
                    }}
                >
                    {paginationLoading && <p>Loading more posts...</p>}
                    {!hasMore && posts.length > 0 && (
                        <p style={{ color: "#999", fontStyle: "italic" }}>No more posts to load</p>
                    )}
                </div>
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