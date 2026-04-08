# ArtPort - Social Media Platform

A full-stack social media application (Instagram / Pintrest clone) built with ASP.NET Core backend and React frontend.

## Backend API Documentation

### Authentication Endpoints

#### POST `/api/auth/register`
Register a new user account.

**Request Body:**
```json
{
  "email": "string (required, valid email)",
  "password": "string (required, min 8 characters)",
  "username": "string (required, 4-20 characters)"
}
```

**Response:**
```json
{
  "message": "User created successfully",
  "userId": 123,
  "token": "jwt_token_string"
}
```

**Status Codes:**
- `200` - Success
- `400` - Email or username already taken

#### POST `/api/auth/login`
Authenticate and login a user.

**Request Body:**
```json
{
  "email": "string (required)",
  "password": "string (required)"
}
```

**Response:**
```json
{
  "userId": 123
}
```

**Status Codes:**
- `200` - Success
- `401` - Invalid credentials

### Post Endpoints

#### GET `/api/post/feed`
Get the feed of posts from users the authenticated user follows.

**Query Parameters:**
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:**
```json
{
  "posts": [
    {
      "id": 1,
      "user": {
        "id": 123,
        "username": "string",
        "profilePictureUrl": "string (optional)"
      },
      "photoUrl": "string",
      "caption": "string (optional)",
      "createdAt": "2024-01-01T00:00:00Z",
      "updatedAt": "2024-01-01T00:00:00Z",
      "likesCount": 42,
      "commentsCount": 7
    }
  ]
}
```

**Authorization:** Required (JWT token)

#### GET `/api/post/user`
Get posts from the authenticated user.

**Query Parameters:**
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:** Same as `/api/post/feed`

**Authorization:** Required (JWT token)

#### POST `/api/post`
Create a new post with an image.

**Request:** `multipart/form-data`
- `caption` (optional): string
- `image`: file (required, image file)

**Response:** `200 OK` on success

**Authorization:** Required (JWT token)

### Profile Endpoints

#### GET `/api/profile`
Get the authenticated user's profile information.

**Response:**
```json
{
  "username": "string",
  "bio": "string (optional)",
  "profilePictureUrl": "string (optional)"
}
```

**Authorization:** Required (JWT token)

#### PUT `/api/profile`
Update the authenticated user's profile information.

**Request Body:**
```json
{
  "username": "string (4-20 characters)",
  "bio": "string (max 500 characters, optional)",
  "profilePictureUrl": "string (optional)"
}
```

**Response:** Updated profile data

**Authorization:** Required (JWT token)

#### POST `/api/profile/profile-image`
Upload a new profile image for the authenticated user.

**Request:** `multipart/form-data`
- `file`: image file (required)

**Response:**
```json
{
  "profilePictureUrl": "string"
}
```

**Authorization:** Required (JWT token)

### Comment Endpoints

#### GET `/api/comment/{postId}`
Get comments for a specific post.

**Query Parameters:**
- `page` (optional, default: 1)
- `pageSize` (optional, default: 20)

**Response:**
```json
{
  "comments": [
    {
      "commentId": 1,
      "userId": 123,
      "postId": 456,
      "content": "string",
      "createdAt": "2024-01-01T00:00:00Z"
    }
  ]
}
```

**Authorization:** Required (JWT token)

#### POST `/api/comment`
Create a new comment on a post.

**Request Body:**
```json
{
  "userId": 123,
  "postId": 456,
  "content": "string (required)"
}
```

**Response:**
```json
{
  "commentId": 789,
  "userId": 123,
  "postId": 456,
  "content": "string",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

**Authorization:** Required (JWT token)

## Authentication

All endpoints except registration and login require authentication via JWT token. The token should be included in the `Authorization` header as a Bearer token, or it will be automatically read from the `auth_token` cookie set during login.

## Data Models

### User
- `id`: integer (primary key)
- `email`: string (unique)
- `username`: string (unique, 4-20 characters)
- `password_hash`: string (bcrypt hash)
- `created_at`: timestamp
- `updated_at`: timestamp
- `bio`: string (optional, max 500 characters)
- `profile_picture_url`: string (optional)

### Post
- `id`: integer (primary key)
- `user_id`: integer (foreign key)
- `image_url`: string
- `caption`: string (optional)
- `created_at`: timestamp
- `updated_at`: timestamp

### Comment
- `id`: integer (primary key)
- `post_id`: integer (foreign key)
- `user_id`: integer (foreign key)
- `content`: string (max 1000 characters)
- `created_at`: timestamp

### Like
- `id`: integer (primary key)
- `post_id`: integer (foreign key)
- `created_at`: timestamp

## Database Schema

The application uses PostgreSQL with the following tables:
- `users`
- `posts`
- `comments`
- `likes`

## Getting Started

1. Ensure PostgreSQL is running
2. Update connection string in `secrets.json`
3. Run database migrations (if using EF Core migrations)
4. Start the backend: `dotnet run`
5. Start the frontend: `npm run dev`

## Technologies Used

- **Backend:** ASP.NET Core, Entity Framework Core, PostgreSQL, JWT Authentication
- **Frontend:** React, TypeScript, Vite
- **Authentication:** JWT tokens with cookie-based storage