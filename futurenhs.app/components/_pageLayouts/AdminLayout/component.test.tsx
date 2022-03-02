import { render, screen } from '@testing-library/react';

import { AdminLayout } from './index';

import { Props } from './interfaces';

describe('Admin Layout', () => {
    
    const props: Props = {

        contentText: {
            mainHeading: 'Mock heading'
        }

    }

    it('renders correctly', () => {
        
        render(<AdminLayout {...props}/>);

        expect(screen.getAllByText('Mock heading').length).toBe(1);

    });

});