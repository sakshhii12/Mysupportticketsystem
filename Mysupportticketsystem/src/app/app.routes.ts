import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
// We will convert these components to standalone in the next step
import { Layout } from './core/layout/layout';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { TicketList } from './features/tickets/ticket-list/ticket-list';
import { AnalyticsDashboard } from './features/dashboard/analytics-dashboard/analytics-dashboard';

// We'll add the authGuard later
// import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Routes accessible to everyone
  { path: 'login', component: Login },
  { path: 'register', component: Register },

  // Routes protected by the main layout
  {
    path: '',
    component: Layout,
    canActivate: [authGuard], // We'll enable this later
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AnalyticsDashboard },
      { path: 'tickets', component: TicketList },
    ]
  },

  // If no other route matches, redirect to the login page
  { path: '**', redirectTo: 'login' }
];
