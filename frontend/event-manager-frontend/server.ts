import 'zone.js/node';
import { APP_BASE_HREF } from '@angular/common';
import { CommonEngine } from '@angular/ssr';
import express from 'express';
import { fileURLToPath } from 'node:url';
import { dirname, join, resolve } from 'node:path';
import { createProxyMiddleware } from 'http-proxy-middleware';

import bootstrap from './src/main.server';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const browserDistFolder = resolve(__dirname, '../browser');
const serverDistFolder = resolve(__dirname, '../server');
const indexHtml = join(serverDistFolder, 'index.server.html');

const app = express();
const commonEngine = new CommonEngine();

const apiProxy = createProxyMiddleware({
  target: 'http://event_api:80',
  changeOrigin: true,
  pathRewrite: { '^/api': '/api' },
});
app.use('/api', apiProxy);

app.use(
  '/api',
  (
    err: any,
    req: express.Request,
    res: express.Response,
    next: express.NextFunction
  ) => {
    console.error('Proxy error:', err);
    res.status(502).send('Bad Gateway');
  }
);

app.get(
  '*.*',
  express.static(browserDistFolder, { maxAge: '1y', immutable: true })
);

app.get('*', async (req, res, next) => {
  try {
    const html = await commonEngine.render({
      bootstrap,
      documentFilePath: indexHtml,
      url: `${req.protocol}://${req.headers.host}${req.originalUrl}`,
      publicPath: browserDistFolder,
      providers: [{ provide: APP_BASE_HREF, useValue: req.baseUrl }],
    });
    res.send(html);
  } catch (err) {
    console.error('SSR render error:', err);
    next(err);
  }
});

const PORT = process.env['PORT'] || 4000;
app.listen(PORT, () => {
  console.log(`SSR Server running on http://localhost:${PORT}`);
});
