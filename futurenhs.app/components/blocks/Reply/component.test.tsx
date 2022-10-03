import { render, screen, cleanup } from "@jestMocks/index";

import { Reply } from './index';

import { Props } from "./interfaces";

describe('Reply', () => {
    
    const props: Props = {
        targetId: '123',
        csrfToken: '789',
        submitAction: null,
        text: {
            reply: 'Reply'
        }
    }

    it('Renders correctly', () => {

        render(<Reply {...props} />)

        expect(screen.getAllByText('Reply').length).toBeGreaterThan(0)

    })

});