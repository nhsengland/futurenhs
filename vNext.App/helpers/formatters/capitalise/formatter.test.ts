import { capitalise } from './index';

describe('Capitalise formatter', () => {

    it('Should convert the first character of a string to uppercase', () => {

        expect(capitalise({ 
            value: 'mock' 
        })).toBe('Mock');

    });

});
