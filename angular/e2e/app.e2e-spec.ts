import { GeekathonAutoSyncTemplatePage } from './app.po';

describe('GeekathonAutoSync App', function() {
  let page: GeekathonAutoSyncTemplatePage;

  beforeEach(() => {
    page = new GeekathonAutoSyncTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
