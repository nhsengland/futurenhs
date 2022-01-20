import React from 'react';
import classNames from 'classnames';
import { ErrorSummary } from '@components/ErrorSummary';

import { Props, State } from './interfaces';

export class ErrorBoundary extends React.PureComponent<Props, State> {

    static defaultProps = {
        text: {
            error: 'Something went wrong'
        }
    };

    /**
     * Class Constructor
     */
    constructor(props?: Props) {

        super(props);

        this.state = {
            hasError: false
        }

    }

    /**
     * Check component has recieved error
     */
    public componentDidCatch(error: any): void {

        console.error(error);
        console.error(this.props);

        this.setState({
            hasError: error
        });

    }

    /**
     * Component render method
     */
    public render() {

        const { text, children, className } = this.props;
        const { error } = text;
        const { hasError } = this.state;

        const generatedClasses: any = {
            wrapper: classNames('c-error-boundary', className)
        };

        if(hasError){

            return (
            
                <ErrorSummary text={{
                    body: error
                }} className={generatedClasses.wrapper} />

            )

        }

        return React.Children.count(children) > 0 ? children : null

    }

}
