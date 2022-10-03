import { render, screen, cleanup } from "@jestMocks/index";

import { Props } from "./interfaces";

import { GroupTeaser } from "./index";

describe('Group teaser', () => {

    const props: Props = {
        text: {
            mainHeading: 'Test main heading',
            strapLine: 'Strapline'
        }
    }

    
    it('Renders correctly', () => {

        render(<GroupTeaser {...props} />)

        expect(screen.getAllByText('Test main heading').length).toBe(1)

    })

});