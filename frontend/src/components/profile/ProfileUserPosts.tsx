import { useEffect, useState } from "react";

type PostItem = {
    id: number;
    user: {
        id: number;
        username: string;
        profilePictureUrl?: string;
    };
    photoUrl: string;
    caption?: string;
    createdAt: string;
    updatedAt: string;
    likesCount: number;
    commentsCount: number;
};

export default function ProfileUserPosts() {
    const [posts, setPosts] = useState<PostItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showNewPost, setShowNewPost] = useState(false);
    const [caption, setCaption] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const [submitting, setSubmitting] = useState(false);

    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

    const fetchUserPosts = async () => {
        setLoading(true);
        setError(null);

        try {
            const res = await fetch(`${apiBaseUrl}/api/post/user`, {
                method: "GET",
                credentials: "include",
            });

            if (!res.ok) {
                throw new Error(`Fetch failed (${res.status})`);
            }

            const data = await res.json();
            setPosts(data.posts);
        } catch (err) {
            console.error(err);
            setError("Failed to load user posts");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUserPosts();
    }, [apiBaseUrl]);

    const handleNewPost = async () => {
        if (!file) {
            setError("Please choose an image file.");
            return;
        }

        setSubmitting(true);
        setError(null);

        try {
            const formData = new FormData();
            formData.append("file", file);
            formData.append("caption", caption);

            const res = await fetch(`${apiBaseUrl}/api/post`, {
                method: "POST",
                credentials: "include",
                body: formData,
            });

            if (!res.ok) {
                throw new Error(`Create post failed (${res.status})`);
            }

            setCaption("");
            setFile(null);
            setShowNewPost(false);
            await fetchUserPosts();
        } catch (err) {
            console.error(err);
            setError("Failed to create post");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div style={{ maxWidth: 800, marginTop: 24 }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <h2>Your Posts</h2>
                <button onClick={() => setShowNewPost((prev) => !prev)}>
                    {showNewPost ? "Cancel" : "New Post"}
                </button>
            </div>

            {showNewPost && (
                <div style={{ border: "1px solid #ddd", padding: 16, borderRadius: 8, marginBottom: 16 }}>
                    <div style={{ marginBottom: 8 }}>
                        <label>
                            Image<span style={{ color: "red" }}>*</span> <br />
                            <input type="file" accept="image/*" onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
                        </label>
                    </div>
                    <div style={{ marginBottom: 8 }}>
                        <label>
                            Caption <br />
                            <textarea value={caption} onChange={(e) => setCaption(e.target.value)} rows={3} style={{ width: "100%" }} />
                        </label>
                    </div>
                    <button onClick={handleNewPost} disabled={submitting}>
                        {submitting ? "Posting..." : "Create Post"}
                    </button>
                </div>
            )}

            {loading && <div>Loading posts...</div>}
            {error && <div style={{ color: "red" }}>{error}</div>}

            {!loading && !error && posts.length === 0 && <div>No posts found.</div>}

            <div style={{ display: "grid", gap: 16 }}>
                {posts.map((post) => (
                    <div key={post.id} style={{ border: "1px solid #ccc", borderRadius: 8, overflow: "hidden" }}>
                        <img src={`${apiBaseUrl}${post.photoUrl}`} alt="post" style={{ width: "100%", height: 320, objectFit: "cover" }} />
                        <div style={{ padding: 12 }}>
                            <div style={{ fontSize: 14, color: "#555", marginBottom: 8 }}>
                                {new Date(post.createdAt).toLocaleString()} • {post.likesCount} likes • {post.commentsCount} comments
                            </div>
                            <div>{post.caption}</div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}
