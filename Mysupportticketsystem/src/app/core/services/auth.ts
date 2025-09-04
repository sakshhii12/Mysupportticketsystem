import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  private apiUrl = '/api/auth'; 
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  public currentUserId$ = new BehaviorSubject<string | null>(this.getUserId());
  constructor(private http: HttpClient, private router: Router) { }

  // Method to handle user login
  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => this.setTokens(response.token, response.refreshToken))
    );
  }

  // Method to handle user registration
  register(userInfo: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userInfo, { responseType: 'text' });
  }

  // Method to log the user out
  logout(): void {
    localStorage.clear(); 
    this.currentUserId$.next(null);
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.router.navigate(['/login']);
  }

  private getUserRoles(): string[] | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    const payload = JSON.parse(atob(token.split('.')[1]));
    const roles = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (!roles) {
      return [];
    }
    return Array.isArray(roles) ? roles : [roles];
  }

  public isAdmin(): boolean {
    const roles = this.getUserRoles();
    return roles ? roles.includes('Admin') : false;
  }

  public isAgent(): boolean {
    const roles = this.getUserRoles();
    return roles ? roles.includes('Agent') : false;
  }
  // Store tokens in localStorage
  private setTokens(token: string, refreshToken: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
    this.currentUserId$.next(this.getUserId());
  }

  // Get the JWT from storage
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // Check if the user is currently authenticated
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
  public getUserId(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      
      return payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    } catch (e) {
      return null;
    }
  }

  public addAuthBackground(): void {
    document.body.classList.add('auth-background');
  }
  public removeAuthBackground(): void {
    document.body.classList.remove('auth-background');
  }
} 
