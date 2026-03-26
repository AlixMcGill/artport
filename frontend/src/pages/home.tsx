import { useState } from "react"
import { Navigate } from "react-router-dom";

export default function Home() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const res = await fetch("/api/auth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({email, password}),
            });

        if (!res.ok) {
            throw new Error("Invalid Credentials");
        }

        const data = await res.json();

        localStorage.setItem("token", data.token);

        Navigate("/dashboard")

        } catch (err: any) {
            setError(err.message || "Login Failed");
        } finally {
            setLoading(false);
        }
    }

    return(
        <>
            <p>Home page</p>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                    <label>Email</label>
                    <input
                        type="email"
                        placeholder="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                    <br/>
                    <label>password</label>
                    <input
                        type="password"
                        placeholder="password"
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <br/>
                    {error && <p style={{color: "red"}}>{error}</p>}
                    <br/>
                    <button type="submit" disabled={loading}> 
                        {loading ? "Loading..." : "Login"}
                    </button>
            </form>
        </>
    )
}
