import { useEffect, useState } from "react";

type UserProfile = {
    username: string;
    bio?: string;
    profilePictureUrl?: string;
};

export default function ProfileHeader() {
    const [user, setUser] = useState<UserProfile | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [saving, setSaving] = useState<boolean>(false);
    const [uploading, setUploading] = useState<boolean>(false);

    const [username, setUsername] = useState<string>("");
    const [bio, setBio] = useState<string>("");
    const [file, setFile] = useState<File | null>(null);

    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                setLoading(true);
                const res = await fetch(`${apiBaseUrl}/api/profile/profile`, {
                    method: "GET",
                    credentials: "include",
                });

                if (!res.ok) {
                    throw new Error(`Request Failed: ${res.status}`);
                }

                const data: UserProfile = await res.json();
                setUser(data);
                setUsername(data.username);
                setBio(data.bio || "");
            } catch (err) {
                console.error(err);
                setError("Failed to load profile");
            } finally {
                setLoading(false);
            }
        };

        fetchProfile();
    }, [apiBaseUrl]);

    const handleUpdate = async () => {
        if (!user) return;

        try {
            setSaving(true);
            const res = await fetch(`${apiBaseUrl}/api/profile/update`, {
                method: "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ username, bio }),
            });

            if (!res.ok) {
                throw new Error(`Update failed: ${res.status}`);
            }

            const updated = await res.json();
            setUser({
                username: updated.username,
                bio: updated.bio,
                profilePictureUrl: updated.profilePictureUrl || user.profilePictureUrl,
            });
            setError(null);
        } catch (err) {
            console.error(err);
            setError("Failed to update profile");
        } finally {
            setSaving(false);
        }
    };

    const handleUpload = async () => {
        if (!file) {
            setError("Please select an image file first.");
            return;
        }

        try {
            setUploading(true);
            const formData = new FormData();
            formData.append("file", file);

            const res = await fetch(`${apiBaseUrl}/api/profile/update/profile-image`, {
                method: "POST",
                credentials: "include",
                body: formData,
            });

            if (!res.ok) {
                throw new Error(`Upload failed: ${res.status}`);
            }

            const updated = await res.json();
            setUser((prev) => prev && ({
                ...prev,
                profilePictureUrl: updated.profilePictureUrl,
            }));
            setError(null);
        } catch (err) {
            console.error(err);
            setError("Failed to upload profile picture");
        } finally {
            setUploading(false);
        }
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <div style={{ color: "red" }}>{error}</div>;
    if (!user) return <div>No profile found</div>;

    return (
        <div style={{ maxWidth: 600 }}>
            <h1>Profile Header</h1>
            <div style={{ display: "flex", gap: 16, alignItems: "flex-start" }}>
                <img
                    src={`${apiBaseUrl}${user.profilePictureUrl || "/default-avatar.png"}`}
                    alt="profile"
                    width={150}
                    height={150}
                    style={{ objectFit: "cover", borderRadius: 8, border: "1px solid #ccc" }}
                />
                <div style={{ flex: 1 }}>
                    <label style={{ display: "block", marginBottom: 8 }}>
                        Username
                        <input
                            type="text"
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            style={{ width: "100%", marginTop: 4 }}
                        />
                    </label>

                    <label style={{ display: "block", marginBottom: 8 }}>
                        Bio
                        <textarea
                            value={bio}
                            onChange={(e) => setBio(e.target.value)}
                            rows={4}
                            style={{ width: "100%", marginTop: 4 }}
                        />
                    </label>

                    <button onClick={handleUpdate} disabled={saving}>
                        {saving ? "Saving..." : "Save username / bio"}
                    </button>

                    <div style={{ marginTop: 16 }}>
                        <input
                            type="file"
                            accept="image/*"
                            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
                        />
                        <button onClick={handleUpload} disabled={uploading || !file} style={{ marginLeft: 8 }}>
                            {uploading ? "Uploading..." : "Upload profile picture"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}
