# Security and Authorization - Komfy Library Management System

## Admin-Only Features

The following controllers and endpoints are restricted to **Admin users only**:

### 1. Analytics Controller
**Location**: `ASI.Basecode.WebApp/Controllers/AnalyticsController.cs`

**Authorization**: `[Authorize(Roles = "Admin")]` at controller level

**Endpoints**:
- `GET /Analytics/Dashboard` - Analytics dashboard with summary statistics
- `GET /Analytics/MostBorrowed` - Most borrowed books report
- `GET /Analytics/MostViewed` - Most viewed books report
- `GET /Analytics/TopRated` - Top-rated books report
- `GET /Analytics/TopBorrowers` - Top borrowers report

**Reason**: Contains sensitive business intelligence and user behavior data.

---

### 2. Reporting Controller
**Location**: `ASI.Basecode.WebApp/Controllers/ReportingController.cs`

**Authorization**: `[Authorize(Roles = "Admin")]` at controller level

**Endpoints**:
- `GET /Reporting/Index` - Reports landing page
- `GET /Reporting/BorrowingReport` - Detailed borrowing report
- `GET /Reporting/InventoryReport` - Inventory breakdown report
- `GET /Reporting/MemberReport` - Member statistics report

**Reason**: Contains comprehensive operational data and member information.

---

### 3. User Management (Admin Functions)
**Location**: `ASI.Basecode.WebApp/Controllers/UserController.cs`

**Authorization**: `[Authorize(Roles = "Admin")]` at controller level (ALL endpoints are admin-only)

**Protected Endpoints**:
- `GET /User/Index` - List all users
- `GET /User/Create` - Display user creation form
- `POST /User/Create` - Create new user
- `GET /User/Edit/{id}` - Display user edit form
- `POST /User/Edit/{id}` - Update user details
- `POST /User/Delete/{id}` - Delete a user
- `GET /User/Details/{id}` - View user details
- `POST /User/GrantAdmin/{id}` - Grant admin access to a user
- `POST /User/RevokeAdmin/{id}` - Revoke admin access from a user

**Reason**: Complete user management should be restricted to administrators only.

---

## Public/Member Features

The following features are accessible to all authenticated users (Members and Admins):

### Book Catalog
- Search and filter books
- View book details
- View and write reviews
- View ebooks
- Download ebooks

### Borrowing
- Borrow books
- Return books
- View personal borrowing history (`/Borrowing/MyHistory`)

### Account Management
- Edit profile (`/Account/EditProfile`)
- Change password (`/Account/ChangePassword`)
- Logout (`/Account/Logout` or `/Account/SignOutUser`)
- View notifications
- Manage settings (`/UserSetting/MySettings`)

### Chatbot/Recommendations
- Use chatbot for book recommendations (`/Chatbot/Index`)
- Get personalized recommendations (`/Chatbot/Recommendations`)
- View similar books

---

## Anonymous Access

The following features do not require authentication:

### Authentication Pages
- Login (`/Account/Login`)
- Register (`/Account/Register`)
- Forgot Password (`/Account/ForgotPassword`)
- Reset Password (`/Account/ResetPassword`)

### Public Catalog (if enabled)
- Landing page
- Browse books (if configured for public access)

---

## Authorization Configuration

### How It Works

1. **Role-Based Authorization**: Uses ASP.NET Core's built-in `[Authorize(Roles = "Admin")]` attribute
2. **User Roles**: Stored in `Users.Role` field (values: "Admin" or "Member")
3. **Claims-Based**: User role is stored in authentication claims during login

### Checking User Role in Code

To check if current user is admin in a controller:

```csharp
// Check if user has Admin role
var isAdmin = User.IsInRole("Admin");

// Or check the role claim directly
var role = User.FindFirst(ClaimTypes.Role)?.Value;
if (role == "Admin")
{
    // Admin-specific logic
}
```

### Default Accounts

After running the seed migration, these accounts will be available:

**Admin Account**:
- Username: `admin`
- Password: `admin`
- Role: Admin
- Email: admin@komfy.com

**Member Account**:
- Username: `user`
- Password: `user`
- Role: Member
- Email: user@komfy.com

---

## Testing Authorization

### Test Admin Access
1. Login as `admin` / `admin`
2. Navigate to `/Analytics/Dashboard` - Should work ✓
3. Navigate to `/Reporting/BorrowingReport` - Should work ✓

### Test Member Restrictions
1. Login as `user` / `user`
2. Navigate to `/Analytics/Dashboard` - Should be denied ✗
3. Navigate to `/Reporting/BorrowingReport` - Should be denied ✗
4. Try accessing `/User/GrantAdmin/{id}` - Should be denied ✗

### Test Member Access
1. Login as `user` / `user`
2. Navigate to `/Book/Index` - Should work ✓
3. Navigate to `/Borrowing/MyHistory` - Should work ✓
4. Navigate to `/Chatbot/Index` - Should work ✓
5. Navigate to `/Account/EditProfile` - Should work ✓

---

## Security Best Practices Implemented

✓ **Role-Based Access Control (RBAC)** - Separates admin and member privileges
✓ **Password Encryption** - Uses AES encryption for passwords
✓ **Password Reset Tokens** - Time-limited (1 hour) unique tokens
✓ **CSRF Protection** - `[ValidateAntiForgeryToken]` on state-changing operations
✓ **Input Validation** - Model validation with data annotations
✓ **Authentication Required** - Sensitive operations require login

---

## Production Security Recommendations

### High Priority
1. **Enable Authentication** - Uncomment authentication logic in `AccountController.Login()` (lines 96-110)
2. **HTTPS Only** - Enforce HTTPS in production
3. **Strong Passwords** - Implement password complexity requirements
4. **Rate Limiting** - Add rate limiting for login and password reset
5. **Audit Logging** - Log all admin actions (grant/revoke admin, view reports)

### Medium Priority
6. **Two-Factor Authentication (2FA)** - Add 2FA for admin accounts
7. **Session Timeout** - Configure appropriate session timeouts
8. **Password History** - Prevent password reuse
9. **Account Lockout** - Lock accounts after failed login attempts
10. **Email Verification** - Verify email addresses during registration

### Low Priority
11. **API Rate Limiting** - If exposing APIs
12. **Content Security Policy (CSP)** - Add CSP headers
13. **Security Headers** - Add security-related HTTP headers
14. **Penetration Testing** - Conduct security audit

---

## Authorization Summary Table

| Feature | Anonymous | Member | Admin |
|---------|-----------|--------|-------|
| Login/Register | ✓ | ✓ | ✓ |
| Logout | ✗ | ✓ | ✓ |
| Browse Books | ✓* | ✓ | ✓ |
| Book Details | ✓* | ✓ | ✓ |
| Borrow/Return | ✗ | ✓ | ✓ |
| Write Reviews | ✗ | ✓ | ✓ |
| View Ebooks | ✗ | ✓ | ✓ |
| Chatbot | ✗ | ✓ | ✓ |
| Edit Own Profile | ✗ | ✓ | ✓ |
| My History | ✗ | ✓ | ✓ |
| **Analytics** | ✗ | ✗ | ✓ |
| **Reports** | ✗ | ✗ | ✓ |
| **User Management** | ✗ | ✗ | ✓ |
| **Grant/Revoke Admin** | ✗ | ✗ | ✓ |

*Depends on configuration

---

## Need Help?

If you encounter authorization issues:

1. **Check User Role**: Verify the user's role in the database
2. **Clear Cookies**: Clear browser cookies and re-login
3. **Check Claims**: Inspect authentication claims in the debugger
4. **Verify Seed Data**: Ensure admin user was created during migration

---

**Last Updated**: 2025-10-31
**Version**: 1.0