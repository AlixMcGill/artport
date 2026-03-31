import { useState } from "react";

export default function CreateAccount() {
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>();

    const url = import.meta.env.VITE_API_BASE_URL

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        try {
            const res = await fetch(`${url}/api/auth/register`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify({username, email, password})
            });

            if (!res.ok) {
                throw new Error("Invalid Credentials");
            }

            if (res.ok) {
                alert("Account Created")
                // alert is for testing purposes only
            }

            
        } catch (err: any) {
            setError(err.message || "Registration Failed"); 
        } finally {
            setLoading(false);
        }
    }
    return (
        <>
            <form onSubmit={handleSubmit}>
                <label>Username</label>
                <input 
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => {setUsername(e.target.value)}} 
                />
                <br/>
                <label>Email</label>
                <input 
                    type="email" 
                    placeholder="Email"
                    value={email}
                    onChange={(e) => {setEmail(e.target.value)}}
                />
                <br/>
                <label>Password</label>
                <input 
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => {setPassword(e.target.value)}}
                 />
                <br/>
                {error && <p style={{color: "red"}}>{error}</p>}
                <br/>
                <button type="submit" disabled={loading}> 
                    {loading ? "Loading..." : "Login"}
                </button>
            </form>
        </>
    );
}