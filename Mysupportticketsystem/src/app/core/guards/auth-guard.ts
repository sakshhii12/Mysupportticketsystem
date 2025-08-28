import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth'; // Adjusted import path

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    // If the user has a token, allow access.
    return true;
  } else {
    // If not, redirect to the login page and block access.
    router.navigate(['/login']);
    return false;
  }
};
