import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { MatSort } from '@angular/material/sort';
import { AngularMaterialModule } from '../../../angular-material.module';
import { ConfirmDialog } from '../../../shared/confirm-dialog/confirm-dialog';
import { AssignAgentDialog } from '../assign-agent-dialog/assign-agent-dialog';
import { TicketService } from '../ticket';
import { TicketForm } from '../ticket-form/ticket-form';
import { AuthService } from '../../../core/services/auth';
import { debounceTime } from 'rxjs/operators';
//import { MatButton } from '@angular/material/button';
import { HttpParams } from '@angular/common/http';
@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AngularMaterialModule],
  templateUrl: './ticket-list.html',
  styleUrls: ['./ticket-list.css']
})
export class TicketList implements OnInit, AfterViewInit {
  baseDisplayedColumns: string[] = ['id', 'title', 'description', 'status', 'priority', 'userEmail', 'createdAt'];
  filterForm: FormGroup;
  currentUserId: string | null = null;
  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = [];
  isAdmin = false;
  isAgent = false;
  statuses = ['Open', 'InProgress', 'Resolved', 'Closed'];
  priorities = ['Low', 'Medium', 'High'];
  @ViewChild(MatSort) sort!: MatSort;
  public isLoading = false;
  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    public authService: AuthService,
    private cdr: ChangeDetectorRef 
  ) {
    this.filterForm = this.fb.group({
      searchTerm: [''], 
      status: [''],
      priority: ['']
    });
  }

  ngOnInit(): void {
    this.authService.currentUserId$.subscribe(id => {
      this.currentUserId = id;
      if (this.currentUserId) {      
        this.loadTickets();
      } else {
        this.dataSource = new MatTableDataSource<any>([]);
      }
    });
    //this.currentUserId = this.authService.getUserId();

    this.isAdmin = this.authService.isAdmin();
    this.isAgent = this.authService.isAgent();
    this.displayedColumns = [...this.baseDisplayedColumns];
    if (!this.isAgent) {
      this.displayedColumns.push('actions');
    }
    this.loadTickets();
    this.filterForm.valueChanges.pipe(debounceTime(300)).subscribe(() => {
      if (this.currentUserId) {     
        this.loadTickets();
      }
     
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  loadTickets(): void {
    this.isLoading = true;
    this.ticketService.getTickets(this.filterForm.value).subscribe({
      next: (data: any[]) => {
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Failed to load tickets', err);
        this.toastr.error('Could not load ticket data.');
        this.isLoading = false;
      }
    });
  }

  sortFields = [
    { value: 'createdAt', viewValue: 'Creation Date' },
    { value: 'priority', viewValue: 'Priority' },
    { value: 'status', viewValue: 'Status' }
  ];

  sortOrders = [
    { value: 'asc', viewValue: 'Ascending' },
    { value: 'desc', viewValue: 'Descending' }
  ];

  clearFilters(): void {
    this.filterForm.reset({ status: '', priority: '', searchTerm: '' });
  }

  openCreateTicketDialog(): void {
    const dialogRef = this.dialog.open(TicketForm, {
      width: '500px',
      height: '80vh',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.ticketService.createTicket(result).subscribe({
          next: () => {
            this.toastr.success('Ticket created successfully!');
            this.loadTickets();
          },
          error: (err: any) => { this.toastr.error('Failed to create ticket.'); }
        });
      }
    });
  }

  openEditTicketDialog(ticketData: any): void {
    const dialogRef = this.dialog.open(TicketForm, {
      width: '500px',
      disableClose: true,
      height: '80vh',
      data: ticketData
    });
    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.ticketService.updateTicket(ticketData.id, result).subscribe({
          next: () => {
            this.toastr.success('Ticket updated successfully!');
            this.loadTickets();
          },
          error: (err: any) => { this.toastr.error('Failed to update ticket.'); }
        });
      }
    });
  }

  assignTicket(ticketId: number): void {
    const dialogRef = this.dialog.open(AssignAgentDialog, {
      width: '400px',
    });

    dialogRef.afterClosed().subscribe((agentEmail: string) => {
      if (agentEmail) {
        this.ticketService.assignTicket(ticketId, agentEmail).subscribe({
          next: () => {
            this.toastr.success('Ticket assigned successfully!');
            this.loadTickets();
          },
          error: (err: any) => {
            this.toastr.error(err.error || 'Failed to assign ticket.');
          }
        });
      }
    });
  }
  closeTicket(ticket: any): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      width: '400px',
      data: {
        title: 'Confirm Close Ticket',
        message: `Are you sure you want to close ticket #${ticket.id}: "${ticket.title}"?`
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.ticketService.closeTicket(ticket.id).subscribe({
          next: () => {
            this.toastr.success('Ticket closed successfully!');
            this.loadTickets();
          },
          error: (err: any) => {
            this.toastr.error('Failed to close the ticket.');
          }
        });
      }
    });
  }
  canEditOrClose(ticket: any): boolean {
   const currentId = this.authService.getUserId();
   
    return !this.authService.isAgent() && (this.authService.isAdmin() || (currentId === ticket.userId));
  }

  canAssign(): boolean {
    return this.isAdmin;
  }

  // In the TicketList class

  exportAsCsv(): void {
    const formValues = this.filterForm.value;
    let params = new HttpParams();
    if (formValues.status) params = params.append('status', formValues.status);
    if (formValues.priority) params = params.append('priority', formValues.priority);
    if (formValues.searchTerm) params = params.append('searchTerm', formValues.searchTerm);

    this.ticketService.exportTickets(params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = `TicketExport_${new Date().toISOString().slice(0, 10)}.csv`; 
        document.body.appendChild(a);

        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);

        this.toastr.success('CSV export started.');
      },
      error: (err) => {
        this.toastr.error('Failed to export CSV.');
      }
    });
  }

  
}

