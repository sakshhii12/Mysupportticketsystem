import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = '/api/tickets';
  private adminApiUrl = '/api/admin';
  constructor(private http: HttpClient) { }

  getTickets(filterParams: any): Observable<any[]> {
    let params = new HttpParams();

    
    if (filterParams.status) params = params.append('status', filterParams.status);
    if (filterParams.priority) params = params.append('priority', filterParams.priority);
    if (filterParams.searchTerm) params = params.append('searchTerm', filterParams.searchTerm);
    if (filterParams.sortBy) params = params.append('sortBy', filterParams.sortBy);
    if (filterParams.sortOrder) params = params.append('sortOrder', filterParams.sortOrder);

    return this.http.get<any[]>(this.apiUrl, { params });
  }
  createTicket(ticketData: any): Observable<any> {
    return this.http.post(this.apiUrl, ticketData);
  }

  getDashboardAnalytics(): Observable<any> {
   
    return this.http.get<any>('/api/dashboard/analytics');
  }
  updateTicket(id: number, ticketData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}`, ticketData);
  }

  assignTicket(ticketId: number, agentEmail: string): Observable<any> {
    const url = `${this.adminApiUrl}/tickets/${ticketId}/assign/${agentEmail}`;
    return this.http.patch(url, {}, { responseType: 'text' });
  }
  closeTicket(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/close`, {}, { responseType: 'text' });
  }
  exportTickets(params: HttpParams): Observable<Blob> {
    // We tell HttpClient to expect a 'blob' (a file) instead of JSON
    return this.http.get(`${this.apiUrl}/export`, {
      params: params,
      responseType: 'blob'
    });
  }
}
