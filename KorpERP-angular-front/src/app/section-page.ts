import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-section-page',
    styleUrl: './section-page.scss',
    templateUrl: './section-page.html',
})
export class SectionPage {
    private readonly route = inject(ActivatedRoute);
    private readonly data = this.route.snapshot.data;

    protected readonly eyebrow = this.data['eyebrow'];
    protected readonly title = this.data['title'];
    protected readonly description = this.data['description'];
    protected readonly headingId = `${this.route.snapshot.routeConfig?.path || 'inicio'}-title`;
}