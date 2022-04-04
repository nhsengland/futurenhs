import { Props } from './interfaces';
import { GroupContainer } from '.';
import { render } from '@testing-library/react';

const props: Props = {
    className: 'mockGroupContainer',
    children: <p>{"mock group container child text"}</p>
};

describe('Group Container', () => {

    it('renders correctly', () => {
        
        expect(render(<GroupContainer {...props} />).getByText("mock group container child text"));

    });
})