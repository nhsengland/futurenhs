import { lockBodyScroll } from './index';

describe('Dom helpers', () => {

    it('lockBodyScroll ', () => {

        expect(document.body.classList.contains('u-overflow-hidden')).toBe(false);

        lockBodyScroll(true);

        expect(document.body.classList.contains('u-overflow-hidden')).toBe(true);

        lockBodyScroll(false);

        expect(document.body.classList.contains('u-overflow-hidden')).toBe(false);

    });
    
});
