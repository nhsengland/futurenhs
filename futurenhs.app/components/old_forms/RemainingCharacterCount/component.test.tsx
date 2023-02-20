import { render, screen, cleanup } from "@jestMocks/index";

import { RemainingCharacterCount } from './index';

import { Props } from "./interfaces";

describe('Remaining character count', () => {
    
    const props: Props = {
        id: '123',
        currentCharacterCount: 0,
        maxCharacterCount: 50,
        remainingCharactersText: 'characters remaining',
        remainingCharactersExceededText: 'characters too many'
    }


    it('Renders correctly', () => {

        render(<RemainingCharacterCount {...props} />)

        expect(screen.getAllByText('50 characters remaining').length).toBe(1)

    })

    it('Displays characters exceeded text if currentCharacterCount is greater than maxCharacterCount', () => {

        const propsCopy: Props = Object.assign({}, props, { currentCharacterCount: 51 })
        render(<RemainingCharacterCount {...propsCopy} />)

        expect(screen.getAllByText('1 characters too many').length).toBe(1)

    })

});