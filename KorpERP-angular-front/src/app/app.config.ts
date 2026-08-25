import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { environment } from '../environments/environment';
import { routes } from './app.routes';
import { NotaDataService } from './notas/nota-data.service';
import { NotaMockService } from './notas/nota-mock.service';
import { NotaService } from './notas/nota.service';
import { ProdutoDataService } from './produtos/produto-data.service';
import { ProdutoMockService } from './produtos/produto-mock.service';
import { ProdutoService } from './produtos/produto.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(),
    provideRouter(routes),
    {
      provide: ProdutoDataService,
      useClass: environment.useProdutoMocks === true ? ProdutoMockService : ProdutoService,
    },
    {
      provide: NotaDataService,
      useClass: environment.useNotasMocks === true ? NotaMockService : NotaService,
    },
  ]
};
