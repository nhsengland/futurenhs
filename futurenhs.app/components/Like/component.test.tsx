import { render, screen, cleanup } from "@jestMocks/index";

import { Like } from './index';

import { Props } from "./interfaces";

describe('Like', () => {

    const props: Props = {
        targetId: '123',
        likeCount: 5,
        likeAction: null,
        text: {
            countSingular: 'Like',
            countPlural: 'Likes',
            like: 'Like item',
            removeLike: 'Remove like'
        }
    }

    it('Renders correctly', () => {

        render(<Like {...props} />)

        expect(screen.getAllByText(/Likes/).length).toBe(1)
    })

    it('Conditionally renders clickable button if enabled', () => {

        render(<Like {...props} />)
        expect(screen.queryByRole('button')).toBeNull()

        cleanup()

        const propsCopy: Props = Object.assign({}, props, { shouldEnable: true })
        render(<Like {...propsCopy} />)
        expect(screen.getAllByRole('button').length).toBe(1)

    })

});