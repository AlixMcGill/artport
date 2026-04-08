import { useEffect, useState } from "react";

type Comment = {
    commentId: number;
    userId: number;
    postId: number;
    content: string;
    createdAt: string;
};

type CommentsModalProps = {
    postId: number;
    isOpen: boolean;
    onClose: () => void;
    userId: number | null;
    apiBaseUrl: string;
};

export default function CommentsModal({ postId, isOpen, onClose, userId, apiBaseUrl }: CommentsModalProps) {
    const [comments, setComments] = useState<Comment[]>([]);
    const [newCommentContent, setNewCommentContent] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        if (isOpen && userId) {
            console.log("Fetching comments for post:", postId, "User:", userId, "API Base:", apiBaseUrl);
            fetchComments();
        }
    }, [isOpen, postId, userId, apiBaseUrl]);

    const fetchComments = async () => {
        setLoading(true);
        setError(null);

        try {
            console.log("Fetching from URL:", `${apiBaseUrl}/api/comment/${postId}?page=1&pageSize=50`);
            const res = await fetch(`${apiBaseUrl}/api/comment/${postId}?page=1&pageSize=50`, {
                method: "GET",
                credentials: "include",
            });

            if (!res.ok) {
                throw new Error(`Failed to fetch comments (${res.status})`);
            }

            const data = await res.json();
            console.log("Comments fetched:", data);
            setComments(data.comments || []);
        } catch (err) {
            console.error("Error fetching comments:", err);
            setError("Failed to load comments");
        } finally {
            setLoading(false);
        }
    };

    const handleCreateComment = async () => {
        if (!newCommentContent.trim() || !userId) {
            console.log("Validation failed - Content:", newCommentContent.trim(), "UserId:", userId);
            return;
        }

        console.log("Creating comment:", { userId, postId, content: newCommentContent, apiBaseUrl });
        setSubmitting(true);
        setError(null);

        try {
            const res = await fetch(`${apiBaseUrl}/api/comment`, {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    userId: userId,
                    postId: postId,
                    content: newCommentContent,
                }),
            });

            if (!res.ok) {
                throw new Error(`Failed to create comment (${res.status})`);
            }

            const newComment = await res.json();
            console.log("Comment created successfully:", newComment);
            setComments([newComment, ...comments]);
            setNewCommentContent("");
        } catch (err) {
            console.error("Error creating comment:", err);
            setError("Failed to create comment");
        } finally {
            setSubmitting(false);
        }
    };

    const handleOverlayClick = (e: React.MouseEvent) => {
        // Only close if clicking directly on the overlay, not on the modal content
        if (e.target === e.currentTarget) {
            onClose();
        }
    };

    if (!isOpen) {
        return null;
    }

    return (
        <div style={{
            position: "fixed",
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: "rgba(0, 0, 0, 0.5)",
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            zIndex: 1000,
        }} onClick={handleOverlayClick}>
            <div style={{
                backgroundColor: "white",
                borderRadius: "8px",
                padding: "20px",
                width: "90%",
                maxWidth: "500px",
                maxHeight: "80vh",
                overflowY: "auto",
            }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "20px" }}>
                    <h2 style={{ margin: 0 }}>Comments</h2>
                    <button
                        onClick={onClose}
                        style={{
                            background: "none",
                            border: "none",
                            fontSize: "1.5em",
                            cursor: "pointer",
                        }}
                    >
                        ✕
                    </button>
                </div>

                {/* Create Comment Section */}
                <div style={{ marginBottom: "20px", paddingBottom: "20px", borderBottom: "1px solid #eee" }}>
                    <textarea
                        value={newCommentContent}
                        onChange={(e) => setNewCommentContent(e.target.value)}
                        placeholder="Write a comment..."
                        style={{
                            width: "100%",
                            padding: "10px",
                            borderRadius: "4px",
                            border: "1px solid #ddd",
                            fontFamily: "inherit",
                            fontSize: "0.95em",
                            minHeight: "60px",
                            boxSizing: "border-box",
                            marginBottom: "10px",
                        }}
                    />
                    <button
                        onClick={handleCreateComment}
                        disabled={!newCommentContent.trim() || submitting}
                        style={{
                            backgroundColor: newCommentContent.trim() && !submitting ? "#007bff" : "#ccc",
                            color: "white",
                            border: "none",
                            padding: "8px 16px",
                            borderRadius: "4px",
                            cursor: newCommentContent.trim() && !submitting ? "pointer" : "not-allowed",
                            fontSize: "0.95em",
                        }}
                    >
                        {submitting ? "Posting..." : "Post Comment"}
                    </button>
                </div>

                {/* Comments List */}
                {error && <p style={{ color: "red", marginBottom: "10px" }}>{error}</p>}
                {loading && <p>Loading comments...</p>}
                {!loading && comments.length === 0 && <p style={{ color: "#666" }}>No comments yet. Be the first to comment!</p>}

                <div>
                    {comments.map((comment) => (
                        <div key={comment.commentId} style={{ marginBottom: "15px", paddingBottom: "15px", borderBottom: "1px solid #eee" }}>
                            <p style={{ margin: "0 0 5px 0", fontWeight: "bold", fontSize: "0.9em" }}>User {comment.userId}</p>
                            <p style={{ margin: "0 0 5px 0" }}>{comment.content}</p>
                            <p style={{ margin: "0", fontSize: "0.8em", color: "#999" }}>
                                {new Date(comment.createdAt).toLocaleDateString()} {new Date(comment.createdAt).toLocaleTimeString()}
                            </p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
