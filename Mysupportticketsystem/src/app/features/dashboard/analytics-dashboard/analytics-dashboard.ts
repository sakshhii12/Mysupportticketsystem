import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, registerables } from 'chart.js';
import { AngularMaterialModule } from '../../../angular-material.module';
import { TicketService } from '../../tickets/ticket';
Chart.register(...registerables);

@Component({
  selector: 'app-analytics-dashboard',
  standalone: true,
  imports: [CommonModule, AngularMaterialModule, BaseChartDirective],
  templateUrl: './analytics-dashboard.html',
  styleUrls: ['./analytics-dashboard.css']
})
export class AnalyticsDashboard implements OnInit {
  isLoading = true;
  totalTickets = 0;
  openTickets = 0;
  resolvedTickets = 0;
  public categoryChartData: any = {
    labels: [],
    datasets: [{
      data: [], label: 'Tickets by Category',
      backgroundColor: [
        '#3f51b5', 
        '#ff4081', 
        '#4caf50', 
        '#ff9800', 
      ] }]
  };
  public priorityChartData: any = {
    labels: [],
    datasets: [{ data: [] }]
  };
  public barChartType: 'bar' = 'bar';
  public pieChartType: 'pie' = 'pie';
  public chartOptions: any = { responsive: true };
  public chartLegend = true;

  constructor(private ticketService: TicketService) { }

  ngOnInit(): void {
    this.ticketService.getDashboardAnalytics().subscribe(data => {
      if (data && data.totalTickets > 0) {
        this.totalTickets = data.totalTickets;
        this.openTickets = data.openTickets;
        this.resolvedTickets = data.resolvedTickets;


        const categoryData = { ...this.categoryChartData };
        categoryData.labels = Object.keys(data.ticketsByCategory);
        categoryData.datasets[0].data = Object.values(data.ticketsByCategory);
        this.categoryChartData = categoryData;

        const priorityData = { ...this.priorityChartData };
        priorityData.labels = Object.keys(data.ticketsByPriority);
        priorityData.datasets[0].data = Object.values(data.ticketsByPriority);
        this.priorityChartData = priorityData;

        this.isLoading = false;
      } else {
        this.isLoading = false;
      }
    });
  }
}
