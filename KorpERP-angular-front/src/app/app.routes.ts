import { Routes } from '@angular/router';
import { SectionPage } from './section-page';
import { Produtos } from './produtos/produtos';
import { Notas } from './notas/notas';

export const routes: Routes = [
    {
        path: '',
        component: SectionPage,
        data: {
            eyebrow: 'Visao geral',
            title: 'Bem-vindo ao KorpERP',
            description: 'Selecione uma opcao no menu para comecar.',
        },
    },
    {
        path: 'produtos',
        component: Produtos,
        data: {
            eyebrow: 'Cadastros',
            title: 'Produtos',
            description: 'Consulte e gerencie os produtos cadastrados.',
        },
    },
    {
        path: 'notas',
        component: Notas,
        data: {
            eyebrow: 'Movimentacoes',
            title: 'Notas fiscais',
            description: 'Acompanhe o processamento das notas fiscais.',
        },
    },
    { path: '**', redirectTo: '' },
];
