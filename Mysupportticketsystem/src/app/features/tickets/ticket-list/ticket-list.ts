import { Component, OnInit, ChangeDetectorRef, ViewChild, AfterViewInit } from '@angular/core'; 
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { MatSort } from '@angular/material/sort';
import { AngularMaterialModule } from '../../../angular-material.module';
import { TicketService } from '../ticket';
import { TicketForm } from '../ticket-form/ticket-form';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-ticket-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, AngularMaterialModule],
  templateUrl: './ticket-list.html',
  styleUrls: ['./ticket-list.css']
})
export class TicketList implements OnInit, AfterViewInit {
  filterForm: FormGroup;
  dataSource = new MatTableDataSource<any>();
  displayedColumns: string[] = ['id', 'title', 'status', 'priority', 'userEmail', 'createdAt', 'actions'];
  isAdmin = false;
  statuses = ['Open', 'InProgress', 'Resolved', 'Closed'];
  priorities = ['Low', 'Medium', 'High'];
  @ViewChild(MatSort) sort!: MatSort;
  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    public dialog: MatDialog,
    private toastr: ToastrService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef 
  ) {
    this.filterForm = this.fb.group({
      status: [''],
      priority: [''],
      searchTerm: [''],
      sortBy: ['createdAt'],
      sortOrder:['desc']
    });
  }

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.loadTickets();
    this.filterForm.valueChanges.subscribe(() => {
      this.loadTickets();
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
  }

  loadTickets(): void {
    this.ticketService.getTickets(this.filterForm.value).subscribe((data: any[]) => {
      this.dataSource.data = data;
      this.cdr.detectChanges(); 
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
    const dialogRef = this.dialog.open(TicketForm, { width: '500px', disableClose: true });
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
    const dialogRef = this.dialog.open(TicketForm, { width: '500px', disableClose: true, data: ticketData });
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
    const agentEmail = prompt('Enter the email of the agent to assign this ticket to:');
    if (agentEmail) {
      this.ticketService.assignTicket(ticketId, agentEmail).subscribe({
        next: () => {
          this.toastr.success('Ticket assigned successfully!');
          this.loadTickets();
        },
        error: (err: any) => {
          let errorMessage = 'Failed to assign ticket.';
          
          if (err.error && typeof err.error === 'string') {
            errorMessage = err.error;
          }
          this.toastr.error(errorMessage, 'Error');
        }
      });
    }
  }
}
