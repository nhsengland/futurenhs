import { capitalise } from './index';

describe('Capitalise formatter', () => {

    it('Should convert the first character of a string to uppercase', () => {

        expect(capitalise({ 
            value: 'mock' 
        })).toBe('Mock');

    });

    it('Should return non-string values verbatim', () => {

        expect(capitalise({
            value: 12
        })).toBe(12);

        expect(capitalise({
            value: null
        })).toBe(null);

        expect(capitalise({
            value: {}
        })).toStrictEqual({});

        expect(capitalise({
            value: ["Random"]
        })).toStrictEqual(["Random"]);

    });

});
