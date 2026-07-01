# Chat App

A full-stack real-time chat & dating application, built with Clean Architecture on the backend and modern Angular on the frontend. It covers everything a real communication app needs: account registration and authentication, member profiles, likes, real-time messaging over SignalR, and photo uploads through Cloudinary.

The project is split into two completely independent parts:

- **`DatingApp Api`** → the Backend, built with ASP.NET Core.
- **`DatingApp_Client`** → the Frontend, built with Angular.

---

## 🖥️ Backend — `DatingApp Api`

The backend is built on **.NET 10** and split into 3 separate projects within the same solution, following **Clean Architecture** principles, so each layer has a single, clear responsibility with no overlap between them.

### Layer breakdown

```
DatingApp Api/
├── DatingApp.Core/            → Domain and business logic layer
│   ├── Domain/
│   │   ├── Entities/          → Member, Message, Group, Connection, Photo, MemberLike
│   │   ├── IdentityEntities/  → AppUser (built on ASP.NET Identity)
│   │   └── RepositoryContracts/ → Repository interfaces
│   ├── DTO/                   → All Data Transfer Objects
│   ├── ServiceContracts/      → Service interfaces (IMessageService, ILikesService...)
│   ├── Services/              → Actual business logic implementation
│   ├── Helpers/                → Pagination, Params classes
│   └── Settings/              → JWT and Cloudinary configuration
│
├── DatingApp.Infrastructure/  → Data access layer
│   ├── DbContext/             → ApplicationDbContext (EF Core)
│   ├── Repositories/          → Concrete repository implementations
│   └── Migrations/            → EF Core migrations
│
└── DatingApp.UI/               → API layer (Presentation)
    ├── Controllers/           → Endpoints (Account, Members, Messages, Likes, Admin...)
    ├── SignalR/                → MessageHub, PresenceHub, PresenceTracker
    ├── Middlewares/            → Exception handling
    ├── Filters/                → Action filters (e.g. updating last-active timestamp)
    └── ConfigureServicesExtension.cs → All service and DI registration
```

### How the architecture works

- **Core** doesn't know anything about databases or the web — it only defines entities and contracts (interfaces).
- **Infrastructure** actually implements those contracts using Entity Framework Core with SQL Server.
- **UI** is the API layer itself, receiving requests and calling into services without knowing implementation details.

This makes the codebase much easier to unit test and lets you swap out any layer without touching the rest — if we wanted to replace SQL Server with a different database, we'd only change Infrastructure.

### Key backend features

- **Authentication & Authorization**: built on ASP.NET Identity + JWT tokens, with roles (Admin, Moderator, Member).
- **Real-time messaging**: powered by SignalR, with a dedicated `MessageHub` that manages conversations as groups, so if both parties are online in the same chat, messages are instantly marked as read.
- **Presence tracking**: `PresenceHub` tracks who's currently online via `PresenceTracker`, and pushes an instant notification if someone messages you while you're not in that chat.
- **Likes system**: toggle likes on members, with support for viewing who you've liked, who's liked you, and mutual likes.
- **Photo upload**: handled through Cloudinary, with admin approval before photos go live.
- **Pagination & filtering**: every list endpoint (members, messages, likes) supports pagination and filtering by gender, age, and sort order.
- **Global exception handling**: a dedicated middleware catches any exception and returns a consistent response instead of a raw stack trace.
- **Swagger**: automatic API documentation available in development mode.

### Tech stack

| Technology                         | Purpose                  |
| ---------------------------------- | ------------------------ |
| ASP.NET Core (.NET 10)             | Core framework           |
| Entity Framework Core + SQL Server | Database access          |
| ASP.NET Identity                   | User and role management |
| JWT Bearer                         | Authentication           |
| SignalR                            | Real-time communication  |
| Cloudinary                         | Image storage and upload |
| Swagger / Swashbuckle              | API documentation        |

---

## 💻 Frontend — `DatingApp_Client`

The frontend is built with **Angular 21**, using the framework's latest concepts: **Standalone Components** and **Signals** instead of relying fully on RxJS for state management, styled with **Tailwind CSS** and **daisyUI**.

### Project structure

```
DatingApp_Client/src/
├── Core/
│   ├── Services/         → All services (Account, Member, Message, Presence, Likes, Admin...)
│   ├── interceptors/     → JWT interceptor, error interceptor, loading interceptor
│   ├── guard/            → Route guards (protecting pages from unauthenticated access)
│   ├── Pipes/            → Custom pipes
│   └── directives/       → Custom directives
│
├── Features/             → Each feature in its own folder (feature-based structure)
│   ├── account/          → Login and registration
│   ├── members/          → Member profiles and details
│   ├── messages/         → Chat UI and conversations
│   ├── lists/            → Likes lists
│   ├── admin/            → Admin dashboard (user and photo management)
│   └── home/             → Landing page
│
├── Shared/               → Reusable components
│   ├── paginator/, star-button/, text-input/, image-upload/, errors/, delete-button/
│
├── Layout/               → Navbar and overall layout
├── Models/               → TypeScript interfaces (Member, Message, User...)
└── Environments/         → Environment configuration (Base URL, Hub URL)
```

### How real-time messaging works on the frontend

The `MessageService` opens a direct SignalR connection to the backend the moment you open a chat with someone, authenticating on the hub using the current user's token. New messages arrive instantly with no refresh needed — this is driven by `Signals` that automatically update the UI whenever an event comes in from the server (`NewMessage`, `ReceiveMessageThread`).

There's also a separate `PresenceService` responsible only for tracking who's online, and it shows a toast notification if someone sends you a message while you're not on that chat page.

### Key frontend features

- **Standalone Components + Signals**: no traditional NgModules — state management is simpler and faster using Signals instead of BehaviorSubjects in most places.
- **HTTP Interceptors**: three core interceptors — one automatically attaches the JWT to every request, one catches errors and turns them into clear user-facing messages (toast) or dedicated error pages, and one controls the loading indicator.
- **Route guards**: prevent unauthenticated users from reaching protected pages like profiles and messages.
- **Reusable pagination component** usable on any list-based page.
- **Admin dashboard**: a dedicated page for managing users and approving photos.
- **Responsive UI** built with Tailwind CSS and daisyUI.

### Tech stack

| Technology                        | Purpose                             |
| --------------------------------- | ----------------------------------- |
| Angular 21 (Standalone + Signals) | Core framework                      |
| @microsoft/signalr                | Real-time connection to the backend |
| Tailwind CSS + daisyUI            | Styling and UI components           |
| RxJS                              | HTTP requests and streams           |
| TypeScript                        | Primary language                    |

---

## ⚙️ Running the project locally

### Backend

```bash
cd "DatingApp Api/DatingApp.UI"
dotnet restore
dotnet ef database update
dotnet run
```

You'll need to configure the following in `appsettings.json`:

- `ConnectionStrings:DefaultConnection` → your SQL Server connection string
- `JWT:Key`, `JWT:Issuer`, `JWT:Audience` → authentication settings
- `CloudinarySettings` → your Cloudinary account credentials

### Frontend

```bash
cd DatingApp_Client
npm install
npm start
```

The app runs on `http://localhost:4200` by default, and communicates with the API through the base URL configured in `Environments/environment.development.ts`.

---

## 📌 Note

This project was built as a hands-on exercise in applying Clean Architecture, the Repository pattern, and real-time communication in a complete, realistic application — not just isolated theoretical examples.

## 🔗 Links

[![linkedin](https://img.shields.io/badge/linkedin-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/mohamed-ezzeldin-789337256/)
